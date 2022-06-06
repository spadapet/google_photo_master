using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CasCap.Models;
using CasCap.Services;

namespace PhotoMaster
{
    internal class PhotoHelper
    {
        private readonly GooglePhotosService photos;
        private const string SpecialAlbumName = "Can delete to save space";

        public PhotoHelper(GooglePhotosService photos)
        {
            this.photos = photos;
        }

        public async Task Run(CancellationToken token)
        {
            if (!await this.photos.LoginAsync())
            {
                throw new Exception("Unable to log in.");
            }

            HashSet<string> itemsInAlbums = new HashSet<string>();
            HashSet<string> itemsInSpecialAlbum = new HashSet<string>();
            HashSet<string> itemsToAddToSpecialAlbum = new HashSet<string>();
            HashSet<string> itemsToRemoveFromSpecialAlbum = new HashSet<string>();
            List<Album> albums = await this.photos.GetAlbumsAsync(cancellationToken: token);
            Album specialAlbum = albums.FirstOrDefault(a => a.title == PhotoHelper.SpecialAlbumName);

            foreach (Album album in albums)
            {
                token.ThrowIfCancellationRequested();

                Console.WriteLine($"Fetching album: {album.title} ({album.mediaItemsCount} items)");

                List<MediaItem> items = await this.photos.GetMediaItemsByAlbumAsync(album.id, cancellationToken: token);
                foreach (MediaItem item in items)
                {
                    (album == specialAlbum ? itemsInSpecialAlbum : itemsInAlbums).Add(item.id);
                }
            }

            Console.WriteLine($"Fetching all media items");

            List<MediaItem> allItems = await this.photos.GetMediaItemsByDateRangeAsync(new DateTime(2021, 6, 1), DateTime.Now, cancellationToken: token);
            foreach (MediaItem item in allItems)
            {
                if (!itemsInAlbums.Contains(item.id))
                {
                    if (!itemsInSpecialAlbum.Contains(item.id))
                    {
                        if (itemsToAddToSpecialAlbum.Add(item.id))
                        {
                            // Console.WriteLine($"Can be deleted: {item.filename}");
                        }
                    }
                }
                else if (itemsInSpecialAlbum.Contains(item.id))
                {
                    if (itemsToRemoveFromSpecialAlbum.Add(item.id))
                    {
                        // Console.WriteLine($"Should not delete anymore: {item.filename}");
                    }
                }
            }

            if (itemsToAddToSpecialAlbum.Count > 0)
            {
                Console.WriteLine($"Marking {itemsToAddToSpecialAlbum.Count} items that can be deleted");

                if (specialAlbum == null)
                {
                    specialAlbum = await this.photos.GetOrCreateAlbumAsync(PhotoHelper.SpecialAlbumName);
                }

                if (!await this.photos.AddMediaItemsToAlbumAsync(specialAlbum.id, itemsToAddToSpecialAlbum.ToArray()))
                {
                    Console.WriteLine($"Failed to add items");
                }
            }

            if (itemsToRemoveFromSpecialAlbum.Count > 0)
            {
                Console.WriteLine($"Unmarking {itemsToRemoveFromSpecialAlbum.Count} items that shouldn't be deleted anymore");

                if (!await this.photos.RemoveMediaItemsFromAlbumAsync(specialAlbum.id, itemsToRemoveFromSpecialAlbum.ToArray()))
                {
                    Console.WriteLine($"Failed to remove items");
                }
            }
        }
    }
}
