
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Supabase;

namespace MGE_HEROES.Server
{
    public class GameDbContext
    {
        public readonly Client client;
        public GameDbContext(string url, string key)
        {
            client = new Client(url, key, new SupabaseOptions
            {
                AutoConnectRealtime = true
            });
        }
        public Client GetClient() => client;
    }
}
