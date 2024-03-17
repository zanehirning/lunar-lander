using System;
using LunarLander.Storage;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;

namespace LunarLander.Storage
{
    public class HighScoresDAO
    {
        private bool loading = false;
        private bool saving = false;
        public HighScores loadedHighScoresState = null;

        public HighScoresDAO()
        {
        }

        public void saveHighScores(List<int> highScore)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    // Create something to save
                    HighScores highScoresState = new HighScores(highScore);

                    // Yes, I know the result is not being saved, I dont' need it
                    var result = finalizeSaveAsync(highScoresState);
                    result.Wait();
                }
            }
        }

        private async Task finalizeSaveAsync(HighScores state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScores));
                                mySerializer.WriteObject(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Console.Write("Something went wrong, please try again");
                    }
                }
                this.saving = false;
            });
        }


        public void loadHighScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    var result = finalizeLoadAsync();
                    result.Wait();
                }
            }
        }

        private async Task finalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(HighScores));
                                    loadedHighScoresState = (HighScores)mySerializer.ReadObject(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }
                this.loading = false;
            });
        }
    }
}

