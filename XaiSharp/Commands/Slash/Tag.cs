// Rewrite Soon™

/*using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using MySql.Data;
using MySql.Data.MySqlClient;

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
                string connStr = $"server=localhost;user={config.SqlUser};database={config.SqlDb};password={config.SqlPass};Allow User Variables=True";
                MySqlConnection conn = new(connStr);
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new($"select exists(select name from tags where name=@n)", conn);
                    cmd.Parameters.Add(new("@n", name));
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        try
                        {
                            MySqlCommand insertCmd = new($"insert into tags (name,content,userid,created_at) values (@n, @c, {Context.User.Id}, '{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}')", conn);
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
                string connStr = $"server=localhost;user={config.SqlUser};database={config.SqlDb};password={config.SqlPass};Allow User Variables=True";
                MySqlConnection conn = new(connStr);
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new($"select name,userid from tags where name = @n", conn);
                    cmd.Parameters.Add(new("@n", name));
                    var rdr = cmd.ExecuteReader();
                    if (rdr != null && rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string tagName = rdr.GetString(0);
                            ulong tagUser = rdr.GetUInt64(1);

                            if (Context.User.Id != tagUser)
                            {
                                await RespondAsync("**You did not create this tag.**", ephemeral: true);
                                return;
                            }
                        }
                        rdr.Close();
                        MySqlCommand editCmd = new("update tags set content=@c where name=@n", conn);
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
                string connStr = $"server=localhost;user={config.SqlUser};database={config.SqlDb};password={config.SqlPass};Allow User Variables=True";
                MySqlConnection conn = new(connStr);
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new($"select exists(select name from tags where name=@n)", conn);
                    cmd.Parameters.Add(new("@n", name));
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 1)
                    {
                        try
                        {
                            MySqlCommand displayCmd = new($"select content from tags where name=@n", conn);
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
                string connStr = $"server=localhost;user={config.SqlUser};database={config.SqlDb};password={config.SqlPass}";
                MySqlConnection conn = new(connStr);
                try
                {
                    conn.Open();

                    string query = $"select id,name,userid,created_at from tags where `name` = @n";
                    MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.Add(new("n", name));
                    var reader = cmd.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            int tag_id = reader.GetInt32(0);
                            string db_name = reader.GetString(1);
                            ulong userid = reader.GetUInt64(2);
                            DateTimeOffset created = new(reader.GetDateTime(3), new TimeSpan(0, 0, 0));
                            IGuildUser user = Context.Guild.GetUser(userid);
                            EmbedBuilder infoEmbed = new()
                            {
                                Color = (uint)Math.Floor(new Random().NextDouble() * (0xffffff + 1)),
                                Author = new() { Name = db_name }
                            };
                            infoEmbed.AddField("Created at", $"<t:{((DateTimeOffset)created.UtcDateTime).ToUnixTimeSeconds()}:f>", true);
                            infoEmbed.AddField("Author", $"{user.Mention} ({userid})", true);
                            infoEmbed.AddField("ID", $"{tag_id}", true);
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
}*/
