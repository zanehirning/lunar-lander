using System;
using LunarLander.Storage;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;

namespace LunarLander.Storage
{
    public class KeybindingsDAO
    {
        private bool loading = false;
        private bool saving = false;
        public Keybindings loadedKeybindingState = null;

        public KeybindingsDAO() 
        {
        }

        public void saveKeybind(Dictionary<String, Keys> keybindings)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    // Create something to save
                    Keybindings keybindingState = new Keybindings(keybindings);

                    // Yes, I know the result is not being saved, I dont' need it
                    finalizeSaveAsync(keybindingState);
                }
            }
        }

        private async Task finalizeSaveAsync(Keybindings state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("Keybindings.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Keybindings));
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

        public void loadKeybinds()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    // Yes, I know the result is not being saved, I dont' need it
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
                        if (storage.FileExists("Keybindings.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("Keybindings.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(Keybindings));
                                    loadedKeybindingState = (Keybindings)mySerializer.ReadObject(fs);
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

