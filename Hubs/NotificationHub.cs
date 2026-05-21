using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace SporBeslenmeWeb.Hubs
{
    public class NotificationHub : Hub
    {
        // Sistemde o an kaç kişinin açık olduğunu tutacak sayaç
        private static int _onlineKullaniciSayisi = 0;

        // Biri siteye girdiğinde (sekme açtığında) otomatik tetiklenir
        public override async Task OnConnectedAsync()
        {
            _onlineKullaniciSayisi++;
            // Tüm kullanıcılara yeni sayıyı canlı olarak gönder
            await Clients.All.SendAsync("KullaniciSayisiGuncelle", _onlineKullaniciSayisi);
            await base.OnConnectedAsync();
        }

        // Biri siteden çıktığında (sekmeyi kapattığında) otomatik tetiklenir
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _onlineKullaniciSayisi--;
            // Tüm kullanıcılara güncel sayıyı canlı olarak gönder
            await Clients.All.SendAsync("KullaniciSayisiGuncelle", _onlineKullaniciSayisi);
            await base.OnDisconnectedAsync(exception);
        }
    }
}