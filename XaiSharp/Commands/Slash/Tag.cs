using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System.Data.SQLite;

namespace XaiSharp.Commands.Slash
{
    public class Tag : InteractionModuleBase<SocketInteractionContext>
    {
        [Group("tag", "Store plaintext data!!")]
        public class TagGroup : InteractionModuleBase<SocketInteractionContext>
        {

            Config config = Util.ParseConfig(File.ReadAllText("config.yml"));
            [SlashCommand("create", "Create a tag")]
            public async Task HandleCreate([Summary(description: "Name of the tag")] string name, [Summary(description: "Content of the tag")] string content)
            {
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();

                    SQLiteCommand cmd = new($"select exists(select Name from tags where Name=@n)", conn);
                    cmd.Parameters.Add(new("@n", name));
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        try
                        {
                            SQLiteCommand insertCmd = new($"insert into tags (Name,Content,UserId,CreatedAt) values (@n, @c, {Context.User.Id}, '{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}')", conn);
                            Console.WriteLine(name.Substring(0, Math.Min(50, name.Length)));
                            insertCmd.Parameters.Add(new("n", name.Substring(0, Math.Min(50, name.Length))));
                            insertCmd.Parameters.Add(new("c", content.Substring(0, Math.Min(2000, content.Length))));
                            insertCmd.ExecuteNonQuery();
                            await RespondAsync($"Created tag **{name.Substring(0, Math.Min(50, name.Length))}**.");
                        }
                        catch (Exception ex)
                        {
                            await RespondAsync($"```{ex}```", ephemeral: true);
                        }
                    }
                    else
                    {
                        await RespondAsync("**This tag already exists!**", ephemeral: true);
                    }

                }
                catch (Exception ex)
                {
                    await RespondAsync("**No connection to database!**", ephemeral: true);
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
            }

            [SlashCommand("edit", "Edit a tag")]
            public async Task HandleEdit([Summary(description: "Name of the tag")] string name, [Summary(description: "New content of the tag")] string content)
            {
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();

                    SQLiteCommand cmd = new($"select Name,UserId from tags where Name = @n", conn);
                    cmd.Parameters.Add(new("@n", name));
                    var rdr = cmd.ExecuteReader();
                    if (rdr != null && rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string tagName = rdr.GetString(0);
                            long tagUser = rdr.GetInt64(1);

                            if ((long)Context.User.Id != tagUser)
                            {
                                await RespondAsync("**You did not create this tag.**", ephemeral: true);
                                return;
                            }
                        }
                        rdr.Close();
                        SQLiteCommand editCmd = new("update tags set Content=@c where Name=@n", conn);
                        editCmd.Parameters.Add(new("@n", name));
                        editCmd.Parameters.Add(new("@c", content.Substring(0, Math.Min(2000, content.Length))));
                        editCmd.ExecuteNonQuery();
                        await RespondAsync($"Successfully updated tag **{name}**.");
                    }
                    else
                    {
                        await RespondAsync("**No tag found!**", ephemeral: true);
                    }

                }
                catch (Exception ex)
                {
                    await RespondAsync("**No connection to database!**", ephemeral: true);
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
            }

            [SlashCommand("display", "Display a tag's contents")]
            public async Task HandleDisplay([Summary(description: "Name of the tag")] string name)
            {
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();

                    SQLiteCommand cmd = new($"select exists(select Name from tags where Name=@n)", conn);
                    cmd.Parameters.Add(new("@n", name));
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 1)
                    {
                        try
                        {
                            SQLiteCommand displayCmd = new($"select Content from tags where Name=@n", conn);
                            displayCmd.Parameters.Add(new("n", name.Substring(0, Math.Min(50, name.Length))));
                            object dispScal = displayCmd.ExecuteScalar();
                            if (dispScal != null)
                            {
                                await RespondAsync(dispScal.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            await RespondAsync($"```{ex}```", ephemeral: true);
                        }
                    }
                    else
                    {
                        await RespondAsync("**There is no tag with that name!**", ephemeral: true);
                    }

                }
                catch (Exception ex)
                {
                    await RespondAsync("**No connection to database!**", ephemeral: true);
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
            }

            [SlashCommand("info", "Information about a tag")]
            public async Task HandleInfo([Summary(description: "Name of the tag")] string name)
            {
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();

                    string query = $"select Id,Name,UserId,CreatedAt from tags where `name` = @n";
                    SQLiteCommand cmd = new(query, conn);
                    cmd.Parameters.Add(new("n", name));
                    var reader = cmd.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            int tag_id = reader.GetInt32(0);
                            string db_name = reader.GetString(1);
                            long userid = reader.GetInt64(2);
                            DateTimeOffset created = new(reader.GetDateTime(3), new TimeSpan(0, 0, 0));
                            IGuildUser user = Context.Guild.GetUser((ulong)userid);
                            EmbedBuilder infoEmbed = new()
                            {
                                Color = (uint)Math.Floor(new Random().NextDouble() * (0xffffff + 1)),
                                Author = new() { Name = db_name }
                            };
                            infoEmbed.AddField("Created at", $"<t:{((DateTimeOffset)created.UtcDateTime).ToUnixTimeSeconds()}:f>", false);
                            infoEmbed.AddField("Author", $"{user.Mention} ({userid})", false);
                            infoEmbed.AddField("ID", $"{tag_id}", false);
                            Console.WriteLine(userid);
                            await RespondAsync(embed: infoEmbed.Build());
                        }
                        reader.Close();

                    }
                    else
                    {
                        await RespondAsync("**No tag found with that name!**", ephemeral: true);
                    }

                }
                catch (Exception ex)
                {
                    await RespondAsync("**No connection to database!**", ephemeral: true);
                    Console.WriteLine(ex.ToString());
                }
                conn.Close();
            }
        }
    }
}
