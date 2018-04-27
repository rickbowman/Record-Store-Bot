using Discord.Commands;
using ExampleBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExampleBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("commands")]
        public async Task GetCommands()
        {
            await Context.Channel.SendMessageAsync("Current commands:" +
                                                   "\n!hours: posts our store hours" +
                                                   "\n!search: checks our album inventory based on your search" +
                                                   "\n!price: gets the price of an album" +
                                                   "\n!quantity: gets the quantity of an album we have in stock");
        }

        [Command("hours")]
        public async Task GetHours()
        {
            await Context.Channel.SendMessageAsync("Our hours are:" +
                                                   "\nMonday-Thursday: 9:00am - 6:00pm" +
                                                   "\nFriday: 9:00am - 5:00pm" +
                                                   "\nSatuday: 10:00am - 4:00pm" +
                                                   "\nSunday: Closed");
        }

        [Command("search")]
        public async Task Search([Remainder]string description)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://recordstorewebapi.azurewebsites.net/api/");
                HttpResponseMessage response = client.GetAsync("album/").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic items = (JArray)JsonConvert.DeserializeObject(result);
                AlbumList albums = items.ToObject<AlbumList>();
                List<Album> filtered = albums.Where(a => a.Description.Contains(description)).ToList();

                if (filtered.Count > 0)
                {
                    string output = "The albums in our inventory that match your search are:\n";

                    foreach (var album in filtered)
                        output += (album.Description + "\n");

                    await Context.Channel.SendMessageAsync(output);
                }
                else
                    await Context.Channel.SendMessageAsync("There are no albums in our inventory that match your search.");
            }
        }

        [Command("price")]
        public async Task Price([Remainder]string description)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://recordstorewebapi.azurewebsites.net/api/");
                HttpResponseMessage response = client.GetAsync("album/").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic items = (JArray)JsonConvert.DeserializeObject(result);
                AlbumList albums = items.ToObject<AlbumList>();
                List<Album> filtered = albums.Where(a => a.Description == description).ToList();

                if (filtered.Count == 1)
                {
                    string output = description + " costs: " + filtered[0].Price.ToString("c");
                    await Context.Channel.SendMessageAsync(output);
                }
                else
                    await Context.Channel.SendMessageAsync("You can only search one album at a time, or there are no albums in our inventory that match your search.");
            }
        }

        [Command("quantity")]
        public async Task Quantity([Remainder]string description)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://recordstorewebapi.azurewebsites.net/api/");
                HttpResponseMessage response = client.GetAsync("album/").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic items = (JArray)JsonConvert.DeserializeObject(result);
                AlbumList albums = items.ToObject<AlbumList>();
                List<Album> filtered = albums.Where(a => a.Description == description).ToList();

                if (filtered.Count == 1 && filtered[0].Quantity > 0)
                {
                    string output = "We currently have " + filtered[0].Quantity.ToString() + " copies of " + description + " in our inventory.";
                    await Context.Channel.SendMessageAsync(output + "\n\n*Quantity may be inaccurate. Please call us at (920) 123-4567 to confirm we still have the album you're looking for.");
                }
                else if (filtered.Count == 1 && filtered[0].Quantity == 0)
                    await Context.Channel.SendMessageAsync(description + " is currently out of stock, please call us at (920) 123-4567 for an estimate of when it will be back in stock");
                else
                    await Context.Channel.SendMessageAsync("You can only search one album at a time, or there are no albums in our inventory that match your search.");
            }
        }
    }
}
