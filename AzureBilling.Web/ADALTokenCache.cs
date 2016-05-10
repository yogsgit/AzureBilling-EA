//----------------------------------------------------------------------------------------------
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AzureBillingAPI.Web.Models;
using AzureBillingAPI.Data;

namespace AzureBillingAPI.Web
{
    public class ADALTokenCache : TokenCache
    {
         
        string currentUser;
        UserTokenCache Cache;
        
        // constructor
        public ADALTokenCache(string user)
        {
            DataAccess db = new DataAccess();
            // associate the cache to the current user of the web app
            currentUser = user;
            
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;

            // look up the entry in the DB
            //Cache = db.PerUserTokenCacheList.FirstOrDefault(c => c.webUserUniqueId == User);
            Cache = GetCacheFromStorage(currentUser);
            // place the entry in memory
            this.Deserialize((Cache == null) ? null : Cache.CacheBits);
        }

        private UserTokenCache GetCacheFromStorage(string user)
        {
            EntityRepo<UserTokenCache> repo = new EntityRepo<UserTokenCache>();
            return repo.Get(user, "UserTokenCache");
        }

        // clean up the DB
        public override void Clear()
        {
            DataAccess db = new DataAccess();
            base.Clear();
            foreach (var cacheEntry in db.PerUserTokenCacheList)
                db.PerUserTokenCacheList.Remove(cacheEntry);
            db.SaveChanges();
        }

        // Notification raised before ADAL accesses the cache.
        // This is your chance to update the in-memory copy from the DB, if the in-memory version is stale
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            DataAccess db = new DataAccess();
            if (Cache == null)
            {
                // first time access
                Cache = GetCacheFromStorage( currentUser);
            }
            else
            {   // retrieve last write from the DB
                var status = from e in db.PerUserTokenCacheList
                             where (e.webUserUniqueId == currentUser)
                             select new
                             {
                                 LastWrite = e.LastWrite
                             };
                // if the in-memory copy is older than the persistent copy
                if (status.First().LastWrite > Cache.LastWrite)
                //// read from from storage, update in-memory copy
                {
                    Cache = GetCacheFromStorage(currentUser);
                }
            }
            this.Deserialize((Cache == null) ? null : Cache.CacheBits);
        }
        
        // Notification raised after ADAL accessed the cache.
        // If the HasStateChanged flag is set, ADAL changed the content of the cache
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            DataAccess db = new DataAccess();
            // if state changed
            if (this.HasStateChanged)
            {
                // check for an existing entry
                Cache = GetCacheFromStorage(currentUser);
                if (Cache == null)
                {
                    // if no existing entry for that user, create a new one
                    Cache = new UserTokenCache
                    {
                        WebUserUniqueId = currentUser,
                    };
                }

                // update the cache contents and the last write timestamp
                Cache.CacheBits = this.Serialize();
                Cache.LastWrite = DateTime.Now;

                // update the DB with modification or new entry
                WriteUserTokenCache(Cache);
                //db.Entry(Cache).State = Cache.Id == 0 ? EntityState.Added : EntityState.Modified;
                //db.SaveChanges();
                this.HasStateChanged = false;
            }
        }

        private void WriteUserTokenCache(UserTokenCache cache)
        {
            EntityRepo<UserTokenCache> repo = new EntityRepo<UserTokenCache>();
            repo.Insert(new List<UserTokenCache> { cache });
        }

        void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
            // if you want to ensure that no concurrent write take place, use this notification to place a lock on the entry
        }
    }
}