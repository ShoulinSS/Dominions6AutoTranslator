using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using CsvHelper;
using static System.Net.Mime.MediaTypeNames;
using CsvHelper.Configuration;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace DominionsAutoTranslatorRU
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            if (!Directory.Exists("Output/English"))
            {
                Directory.CreateDirectory("Output/English");
            }

            if (!Directory.Exists("Output/English/Spells"))
            {
                Directory.CreateDirectory("Output/English/Spells");
            }

            if (!Directory.Exists("Output/English/Units"))
            {
                Directory.CreateDirectory("Output/English/Units");
            }

            if (!Directory.Exists("Output/English/Weapons"))
            {
                Directory.CreateDirectory("Output/English/Weapons");
            }

            if (!Directory.Exists("Output/English/Armors"))
            {
                Directory.CreateDirectory("Output/English/Armors");
            }

            if (!Directory.Exists("Output/English/MagicItems"))
            {
                Directory.CreateDirectory("Output/English/MagicItems");
            }

            if (!Directory.Exists("Output/Russian"))
            {
                Directory.CreateDirectory("Output/Russian");
            }

            if (!Directory.Exists("Output/Russian/Spells"))
            {
                Directory.CreateDirectory("Output/Russian/Spells");
            }

            if (!Directory.Exists("Output/Russian/Units"))
            {
                Directory.CreateDirectory("Output/Russian/Units");
            }

            if (!Directory.Exists("Output/Russian/Weapons"))
            {
                Directory.CreateDirectory("Output/Russian/Weapons");
            }

            if (!Directory.Exists("Output/Russian/Armors"))
            {
                Directory.CreateDirectory("Output/Russian/Armors");
            }

            if (!Directory.Exists("Output/Russian/MagicItems"))
            {
                Directory.CreateDirectory("Output/Russian/MagicItems");
            }

            if (!File.Exists("Prompt.txt"))
            {
                File.Create("Prompt.txt");
            }

            Program program = new Program();

            while (true)
            {
                int taskNumber = 0;
                Console.WriteLine("1 - Поиск текста для перевода");
                Console.WriteLine("2 - Перевод найденного текста");
                Console.WriteLine("3 - Составление списка команд для создания мода");

                try
                {
                    taskNumber = Convert.ToInt32(Console.ReadLine());
                }
                catch { continue; }

                switch (taskNumber)
                {
                    case 1:
                        program.TextSearch();

                        break;
                    case 2:
                        program.TranslateText().GetAwaiter().GetResult();

                        break;
                    case 3:
                        program.GenerateCommands();

                        break;
                    default: break;

                }
            }
        }

        private static HttpClient client = new HttpClient();

        private void TextSearch()
        {
            try
            {
                string spellsTablePath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\spells.csv");
                string spellDescriptionsPath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\spelldescr");
                string unitsTablePath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\BaseU.csv");
                string unitDescriptionsPath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\unitdescr");
                string weaponsTablePath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\weapons.csv");
                string armorsTablePath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\armors.csv");
                string itemsTablePath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\BaseI.csv");
                string itemDescriptionsPath = Path.Combine(Directory.GetCurrentDirectory(), "gamedata\\itemdescr");

                List<Spells> spellsUntranslated = new List<Spells>();
                List<Units> unitsUntranslated = new List<Units>();
                List<Weapons> weaponsUntranslated = new List<Weapons>();
                List<Armors> armorsUntranslated = new List<Armors>();
                List<MagicItems> magicItemsUntranslated = new List<MagicItems>();

                int counter = 0;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "\t",
                };

                using (var reader = new StreamReader(spellsTablePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var spellsRecords = csv.GetRecords<SpellsRaw>();

                    foreach (var rawspell in spellsRecords)
                    {
                        string file = string.Concat(spellDescriptionsPath, string.Concat(string.Concat("\\", rawspell.name.Replace(" ", "")), ".txt"));

                        try
                        {
                            string spelldesc = File.ReadAllText(file);

                            Spells spell = new Spells();

                            spell.id = rawspell.id;
                            spell.name = rawspell.name;
                            spell.description = spelldesc;

                            spellsUntranslated.Add(spell);
                        }
                        catch
                        {
                            Spells spell = new Spells();

                            spell.id = rawspell.id;
                            spell.name = rawspell.name;
                            spell.description = "";

                            spellsUntranslated.Add(spell);
                        }
                    }

                    string spellsPath = "Output/English/Spells";

                    foreach (var spell in spellsUntranslated)
                    {
                        using (StreamWriter sw = new StreamWriter($"{spellsPath}/{spell.id}.txt"))
                        {
                            sw.Write(string.Concat(spell.id, "|", spell.name, "|", spell.description));

                            counter++;
                        }
                    }

                    Console.WriteLine($"spells: {counter}");
                }

                using (var reader = new StreamReader(unitsTablePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var unitsRecords = csv.GetRecords<UnitsRaw>();

                    foreach (var rawunit in unitsRecords)
                    {
                        int idLength = rawunit.id.Length;
                        string id = rawunit.id;

                        for (int i = 0; i < 4 - idLength; i++)
                        {
                            id = string.Concat("0", id);
                        }

                        string file = string.Concat(unitDescriptionsPath, string.Concat(string.Concat("\\", id), ".txt"));

                        try
                        {
                            string unitdesc = File.ReadAllText(file);

                            Units unit = new Units();

                            unit.id = rawunit.id;
                            unit.name = rawunit.name;
                            unit.description = unitdesc;

                            unitsUntranslated.Add(unit);
                        }
                        catch
                        {
                            Units unit = new Units();

                            unit.id = rawunit.id;
                            unit.name = rawunit.name;
                            unit.description = "";

                            unitsUntranslated.Add(unit);
                        }
                    }

                    counter = 0;

                    string unitsPath = "Output/English/Units";

                    foreach (var unit in unitsUntranslated)
                    {
                        using (StreamWriter sw = new StreamWriter($"{unitsPath}/{unit.id}.txt"))
                        {
                            sw.Write(string.Concat(unit.id, "|", unit.name, "|", unit.description));

                            counter++;
                        }
                    }

                    Console.WriteLine($"units: {counter}");
                }

                using (var reader = new StreamReader(weaponsTablePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var weaponsRecords = csv.GetRecords<WeaponsRaw>();

                    foreach (var rawweapon in weaponsRecords)
                    {
                        string file = string.Concat(itemDescriptionsPath, string.Concat(string.Concat("\\", rawweapon.name.Replace(" ", "")), ".txt"));

                        try
                        {
                            string weapondesc = File.ReadAllText(file);

                            Weapons weapon = new Weapons();

                            weapon.id = rawweapon.id;
                            weapon.name = rawweapon.name;
                            weapon.description = weapondesc;

                            weaponsUntranslated.Add(weapon);
                        }
                        catch
                        {
                            Weapons weapon = new Weapons();

                            weapon.id = rawweapon.id;
                            weapon.name = rawweapon.name;
                            weapon.description = "";

                            weaponsUntranslated.Add(weapon);
                        }
                    }

                    counter = 0;

                    string weaponsPath = "Output/English/Weapons";

                    foreach (var weapon in weaponsUntranslated)
                    {
                        using (StreamWriter sw = new StreamWriter($"{weaponsPath}/{weapon.id}.txt"))
                        {
                            sw.Write(string.Concat(weapon.id, "|", weapon.name, "|", weapon.description));

                            counter++;
                        }
                    }

                    Console.WriteLine($"weapons: {counter}");
                }

                using (var reader = new StreamReader(armorsTablePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var armorsRecords = csv.GetRecords<ArmorsRaw>();

                    foreach (var rawarmor in armorsRecords)
                    {
                        string file = string.Concat(itemDescriptionsPath, string.Concat(string.Concat("\\", rawarmor.name.Replace(" ", "")), ".txt"));

                        try
                        {
                            string armordesc = File.ReadAllText(file);

                            Armors armor = new Armors();

                            armor.id = rawarmor.id;
                            armor.name = rawarmor.name;
                            armor.description = armordesc;

                            armorsUntranslated.Add(armor);
                        }
                        catch
                        {
                            Armors armor = new Armors();

                            armor.id = rawarmor.id;
                            armor.name = rawarmor.name;
                            armor.description = "";

                            armorsUntranslated.Add(armor);
                        }
                    }

                    counter = 0;

                    string armorsPath = "Output/English/Armors";

                    foreach (var armor in armorsUntranslated)
                    {
                        using (StreamWriter sw = new StreamWriter($"{armorsPath}/{armor.id}.txt"))
                        {
                            sw.Write(string.Concat(armor.id, "|", armor.name, "|", armor.description));

                            counter++;
                        }
                    }

                    Console.WriteLine($"armors: {counter}");
                }

                using (var reader = new StreamReader(itemsTablePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var itemsRecords = csv.GetRecords<ItemsRaw>();

                    foreach (var rawitem in itemsRecords)
                    {
                        string file = string.Concat(itemDescriptionsPath, string.Concat(string.Concat("\\", rawitem.name.Replace(" ", "")), ".txt"));

                        try
                        {
                            string itemdesc = File.ReadAllText(file);

                            MagicItems item = new MagicItems();

                            item.id = rawitem.id;
                            item.name = rawitem.name;
                            item.description = itemdesc;

                            magicItemsUntranslated.Add(item);
                        }
                        catch
                        {
                            MagicItems item = new MagicItems();

                            item.id = rawitem.id;
                            item.name = rawitem.name;
                            item.description = "";

                            magicItemsUntranslated.Add(item);
                        }
                    }

                    counter = 0;

                    string itemsPath = "Output/English/MagicItems";

                    foreach (var item in magicItemsUntranslated)
                    {
                        using (StreamWriter sw = new StreamWriter($"{itemsPath}/{item.id}.txt"))
                        {
                            sw.Write(string.Concat(item.id, "|", item.name, "|", item.description));

                            counter++;
                        }
                    }

                    Console.WriteLine($"magic items: {counter}");
                }

                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                Console.WriteLine("");
            }
        }

        private async Task TranslateText()
        {
            try
            {
                client.Timeout = TimeSpan.FromMinutes(10);

                string spellsPathEN = Path.Combine(Directory.GetCurrentDirectory(), "Output\\English\\Spells");
                string unitsPathEN = Path.Combine(Directory.GetCurrentDirectory(), "Output\\English\\Units");
                string weaponsPathEN = Path.Combine(Directory.GetCurrentDirectory(), "Output\\English\\Weapons");
                string armorsPathEN = Path.Combine(Directory.GetCurrentDirectory(), "Output\\English\\Armors");
                string itemsPathEN = Path.Combine(Directory.GetCurrentDirectory(), "Output\\English\\MagicItems");

                string spellsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Spells");
                string unitsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Units");
                string weaponsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Weapons");
                string armorsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Armors");
                string itemsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\MagicItems");

                List<Spells> spellsUntranslated = new List<Spells>();
                List<Units> unitsUntranslated = new List<Units>();
                List<Weapons> weaponsUntranslated = new List<Weapons>();
                List<Armors> armorsUntranslated = new List<Armors>();
                List<MagicItems> magicItemsUntranslated = new List<MagicItems>();

                string[] spellFiles = Directory.GetFiles(spellsPathEN, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in spellFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Spells spell = new Spells();

                    spell.id = parts[0];
                    spell.name = parts[1];
                    spell.description = parts[2];

                    spellsUntranslated.Add(spell);
                }

                string prompt = File.ReadAllText("Prompt.txt");

                string[] spellFilesTranslated = Directory.GetFiles(spellsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                int counter = 0;

                foreach (string file in spellFilesTranslated)
                {
                    counter++;
                }

                foreach (Spells spell in spellsUntranslated)
                {
                    if (counter > 0)
                    {
                        counter--;
                        continue;
                    }

                    string spellNameTranslated = "";

                    if (spell.name != "") {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", spell.name))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        spellNameTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    string spellDescriptionTranslated = "";

                    if (spell.description != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", spell.description))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        spellDescriptionTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    using (StreamWriter sw = new StreamWriter($"{spellsPathRU}/{spell.id}.txt"))
                    {
                        sw.Write(string.Concat(spell.id, "|", spellNameTranslated, "|", spellDescriptionTranslated));

                        Console.WriteLine($"Переведен spell: {spell.id}|{spell.name}");
                    }
                }

                string[] unitFiles = Directory.GetFiles(unitsPathEN, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in unitFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Units unit = new Units();

                    unit.id = parts[0];
                    unit.name = parts[1];
                    unit.description = parts[2];

                    unitsUntranslated.Add(unit);
                }

                string[] unitFilesTranslated = Directory.GetFiles(unitsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                counter = 0;

                foreach (string file in unitFilesTranslated)
                {
                    counter++;
                }

                foreach (Units unit in unitsUntranslated)
                {
                    if (counter > 0)
                    {
                        counter--;
                        continue;
                    }

                    string unitNameTranslated = "";

                    if (unit.name != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", unit.name))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        unitNameTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    string unitDescriptionTranslated = "";

                    if (unit.description != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", unit.description))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        unitDescriptionTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    using (StreamWriter sw = new StreamWriter($"{unitsPathRU}/{unit.id}.txt"))
                    {
                        sw.Write(string.Concat(unit.id, "|", unitNameTranslated, "|", unitDescriptionTranslated));

                        Console.WriteLine($"Переведен unit: {unit.id}|{unit.name}");
                    }
                }

                string[] weaponFiles = Directory.GetFiles(weaponsPathEN, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in weaponFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Weapons weapon = new Weapons();

                    weapon.id = parts[0];
                    weapon.name = parts[1];
                    weapon.description = parts[2];

                    weaponsUntranslated.Add(weapon);
                }

                string[] weaponFilesTranslated = Directory.GetFiles(weaponsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                counter = 0;

                foreach (string file in weaponFilesTranslated)
                {
                    counter++;
                }

                foreach (Weapons weapon in weaponsUntranslated)
                {
                    if (counter > 0)
                    {
                        counter--;
                        continue;
                    }

                    string weaponNameTranslated = "";

                    if (weapon.name != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", weapon.name))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        weaponNameTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    string weaponDescriptionTranslated = "";

                    if (weapon.description != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", weapon.description))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        weaponDescriptionTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    using (StreamWriter sw = new StreamWriter($"{weaponsPathRU}/{weapon.id}.txt"))
                    {
                        sw.Write(string.Concat(weapon.id, "|", weaponNameTranslated, "|", weaponDescriptionTranslated));

                        Console.WriteLine($"Переведен weapon: {weapon.id}|{weapon.name}");
                    }
                }

                string[] armorFiles = Directory.GetFiles(armorsPathEN, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in armorFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Armors armor = new Armors();

                    armor.id = parts[0];
                    armor.name = parts[1];
                    armor.description = parts[2];

                    armorsUntranslated.Add(armor);
                }

                string[] armorFilesTranslated = Directory.GetFiles(armorsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                counter = 0;

                foreach (string file in armorFilesTranslated)
                {
                    counter++;
                }

                foreach (Armors armor in armorsUntranslated)
                {
                    if (counter > 0)
                    {
                        counter--;
                        continue;
                    }

                    string armorNameTranslated = "";

                    if (armor.name != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", armor.name))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        armorNameTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    string armorDescriptionTranslated = "";

                    if (armor.description != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", armor.description))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        armorDescriptionTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    using (StreamWriter sw = new StreamWriter($"{armorsPathRU}/{armor.id}.txt"))
                    {
                        sw.Write(string.Concat(armor.id, "|", armorNameTranslated, "|", armorDescriptionTranslated));

                        Console.WriteLine($"Переведен armor: {armor.id}|{armor.name}");
                    }
                }

                string[] itemFiles = Directory.GetFiles(itemsPathEN, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in itemFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    MagicItems item = new MagicItems();

                    item.id = parts[0];
                    item.name = parts[1];
                    item.description = parts[2];

                    magicItemsUntranslated.Add(item);
                }

                string[] itemFilesTranslated = Directory.GetFiles(itemsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                counter = 0;

                foreach (string file in itemFilesTranslated)
                {
                    counter++;
                }

                foreach (MagicItems item in magicItemsUntranslated)
                {
                    if (counter > 0)
                    {
                        counter--;
                        continue;
                    }

                    string itemNameTranslated = "";

                    if (item.name != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", item.name))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        itemNameTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    string itemDescriptionTranslated = "";

                    if (item.description != "")
                    {
                        var json = new JObject(
                            new JProperty("model", "qwen2.5-7b-instruct"),
                            new JProperty("messages", new JArray(
                                new JObject(new JProperty("role", "system"), new JProperty("content", prompt)),
                                new JObject(new JProperty("role", "user"), new JProperty("content", item.description))
                            )),
                            new JProperty("temperature", 0.3),
                            new JProperty("max_tokens", 1024)
                        ).ToString();

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:1234/v1/chat/completions")
                        {
                            Content = content
                        };

                        request.Headers.Add("Authorization", "Bearer lm-studio");

                        var response = await client.SendAsync(request);

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var doc = JsonDocument.Parse(responseBody);

                        itemDescriptionTranslated = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();
                    }

                    using (StreamWriter sw = new StreamWriter($"{itemsPathRU}/{item.id}.txt"))
                    {
                        sw.Write(string.Concat(item.id, "|", itemNameTranslated, "|", itemDescriptionTranslated));

                        Console.WriteLine($"Переведен item: {item.id}|{item.name}");
                    }
                }

                Console.WriteLine("Перевод завершен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                Console.WriteLine("");
            }
        }

        private void GenerateCommands()
        {
            try
            {
                string spellsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Spells");
                string unitsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Units");
                string weaponsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Weapons");
                string armorsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\Armors");
                string itemsPathRU = Path.Combine(Directory.GetCurrentDirectory(), "Output\\Russian\\MagicItems");

                List<Spells> spellsTranslated = new List<Spells>();
                List<Units> unitsTranslated = new List<Units>();
                List<Weapons> weaponsTranslated = new List<Weapons>();
                List<Armors> armorsTranslated = new List<Armors>();
                List<MagicItems> magicItemsTranslated = new List<MagicItems>();

                string commands = "";

                commands = string.Concat(commands, "\n\n--Spells\n\n");

                string[] spellFiles = Directory.GetFiles(spellsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in spellFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Spells spell = new Spells();

                    spell.id = parts[0];
                    spell.name = parts[1];
                    spell.description = parts[2];

                    spellsTranslated.Add(spell);
                }

                foreach (Spells spell in spellsTranslated)
                {
                    string newSpell = "";

                    if (spell.description == "")
                    {
                        continue;
                    }

                    newSpell = $"#selectspell {spell.id}\n#descr \"{spell.name}\n{spell.description}\"\n#end\n\n";

                    commands = string.Concat(commands, newSpell);
                }

                commands = string.Concat(commands, "\n\n--Units\n\n");

                string[] unitFiles = Directory.GetFiles(unitsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in unitFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Units unit = new Units();

                    unit.id = parts[0];
                    unit.name = parts[1];
                    unit.description = parts[2];

                    unitsTranslated.Add(unit);
                }

                foreach (Units unit in unitsTranslated)
                {
                    string newUnit = "";

                    if (unit.description == "")
                    {
                        continue;
                    }

                    newUnit = $"#selectmonster {unit.id}\n#descr \"{unit.name}\n{unit.description}\"\n#end\n\n";

                    commands = string.Concat(commands, newUnit);
                }

                commands = string.Concat(commands, "\n\n--Weapons\n\n");

                string[] weaponFiles = Directory.GetFiles(weaponsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in weaponFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Weapons weapon = new Weapons();

                    weapon.id = parts[0];
                    weapon.name = parts[1];
                    weapon.description = parts[2];

                    weaponsTranslated.Add(weapon);
                }

                foreach (Weapons weapon in weaponsTranslated)
                {
                    string newWeapon = "";

                    if (weapon.description == "")
                    {
                        continue;
                    }

                    newWeapon = $"#selectweapon {weapon.id}\n#descr \"{weapon.name}\n{weapon.description}\"\n#end\n\n";

                    commands = string.Concat(commands, newWeapon);
                }

                commands = string.Concat(commands, "\n\n--Armor\n\n");

                string[] armorFiles = Directory.GetFiles(armorsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in armorFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    Armors armor = new Armors();

                    armor.id = parts[0];
                    armor.name = parts[1];
                    armor.description = parts[2];

                    armorsTranslated.Add(armor);
                }

                foreach (Armors armor in armorsTranslated)
                {
                    string newArmor = "";

                    if (armor.description == "")
                    {
                        continue;
                    }

                    newArmor = $"#selectarmor {armor.id}\n#descr \"{armor.name}\n{armor.description}\"\n#end\n\n";

                    commands = string.Concat(commands, newArmor);
                }

                commands = string.Concat(commands, "\n\n--Magic items\n\n");

                string[] itemFiles = Directory.GetFiles(itemsPathRU, "*.txt").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f))).ToArray();

                foreach (string file in itemFiles)
                {
                    string raw = File.ReadAllText(file);

                    string[] parts = raw.Split('|');

                    MagicItems item = new MagicItems();

                    item.id = parts[0];
                    item.name = parts[1];
                    item.description = parts[2];

                    magicItemsTranslated.Add(item);
                }

                foreach (MagicItems item in magicItemsTranslated)
                {
                    string newItem = "";

                    if (item.description == "")
                    {
                        continue;
                    }

                    newItem = $"#selectitem {item.id}\n#descr \"{item.name}\n{item.description}\"\n#end\n\n";

                    commands = string.Concat(commands, newItem);
                }

                using (StreamWriter sw = new StreamWriter("Output/Commands.txt"))
                {
                    sw.Write(commands);

                    Console.WriteLine("Команды сформированы");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                Console.WriteLine("");
            }
        }
    }

    public class SpellsRaw
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Spells
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class UnitsRaw
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Units
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class WeaponsRaw
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Weapons
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ArmorsRaw
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Armors
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ItemsRaw
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class MagicItems
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description {  get; set; }
    }
}
