using Foundation;
using Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile.iOS
{
    public class KeyChain_iOS : IKeyChain
    {
        const string serviceId = "Snooker Byb";

        public override bool Add(string key, string value)
        {
            try
            {
                key = key.ToLower();
                if (string.IsNullOrEmpty(key))
                    return false;

                Delete(key);

                if (value == null)
                    return true;

                value = Crypto.Encrypt(value, "KeyChainService_iOS");

                SecStatusCode code = SecKeyChain.Add(new SecRecord(SecKind.GenericPassword)
                {
                    Service = serviceId,
                    Label = serviceId,
                    Account = key,
                    Generic = NSData.FromString(value, NSStringEncoding.UTF8),
                    Accessible = SecAccessible.WhenUnlockedThisDeviceOnly,
                    Synchronizable = false
                });

                bool ok = code == SecStatusCode.Success;
                return ok;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Delete(string key)
        {
            try
            {
                key = key.ToLower();
                if (string.IsNullOrEmpty(key))
                    return;

                SecRecord queryRec = new SecRecord(SecKind.GenericPassword) { Service = serviceId, Label = serviceId, Account = key, Synchronizable = false };
                SecStatusCode code = SecKeyChain.Remove(queryRec);
            }
            catch (Exception)
            {
            }
        }

        public override string Get(string key)
        {
            try
            {
                key = key.ToLower();
                if (string.IsNullOrEmpty(key))
                    return null;

                SecStatusCode code;
                SecRecord queryRec = new SecRecord(SecKind.GenericPassword) { Service = serviceId, Label = serviceId, Account = key, Synchronizable = false };
                queryRec = SecKeyChain.QueryAsRecord(queryRec, out code);

                if (code == SecStatusCode.Success && queryRec != null && queryRec.Generic != null)
                {
                    string value = NSString.FromData(queryRec.Generic, NSStringEncoding.UTF8);
                    value = Crypto.Decrypt(value, "KeyChainService_iOS");
                    return value;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
