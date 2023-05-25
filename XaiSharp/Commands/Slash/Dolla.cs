using Discord.Interactions;
using Discord;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Reactive;

namespace XaiSharp.Commands.Slash
{
    public class Dolla : InteractionModuleBase<SocketInteractionContext>
    {

        [Group("dolla", "Manage your dolla")]
        public class DollaGroup : InteractionModuleBase<SocketInteractionContext>
        {
            Config config = Util.ParseConfig(File.ReadAllText("config.yml"));
            [SlashCommand("tax", "tax someone (shieldbug only)")]
            public async Task HandleTax(IGuildUser user, int amount)
            {
                ulong[] admins =
                {
                    363853381061574658,
                    52635605956506421
                };

                //Console.WriteLine(Context.User.Id);
                if (!admins.Contains(Context.User.Id))
                {
                    await RespondAsync("You do not have the permissions to tax someone", ephemeral: true);
                    return;
                }

                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();
                    //check if the user is in db.
                    SQLiteCommand cmd = new($"select exists(select UserId from Dolla where UserId={user.Id})", conn);
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        try
                        {
                            SQLiteCommand insertCmd = new($"insert into dolla (UserId, Dolla, LastFreeDolla) values ({user.Id}, {0 - amount}, 0)", conn);
                            insertCmd.ExecuteNonQuery();
                            await RespondAsync($"Taxed {user.Username} for Đ{amount}.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            await RespondAsync($"```{ex}```", ephemeral: true);
                        }
                    }
                    else
                    {
                        try
                        {
                            //We need to get the amount of dolla the user already has
                            SQLiteCommand getDollaCmd = new($"select Dolla from Dolla where UserId={user.Id}", conn);
                            object dollaAmountScalar = getDollaCmd.ExecuteScalar();
                            int dollaAmount = 0;
                            if (dollaAmountScalar != null)
                            {
                                dollaAmount = Convert.ToInt32(dollaAmountScalar);
                            }
                            SQLiteCommand updateCmd = new($"update dolla set Dolla={dollaAmount - amount} where UserId={user.Id}", conn);
                            updateCmd.ExecuteNonQuery();
                            await RespondAsync($"Taxed {user.Username} for Đ{amount}.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            await RespondAsync($"```{ex}```", ephemeral: true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
            }
            [SlashCommand("leaderboard", "you can see Who Has The Most Dolla")]
            public async Task HandleLeaderboard(int? page = null)
            {
                Dictionary<IGuildUser, int> list = new();
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                int pageFinal = page - 1 ?? 0;
                try
                {
                    conn.Open();
                    //await DeferAsync();
                    // 100 user limit for now
                    SQLiteCommand cmd = new($"select UserId,Dolla from dolla order by dolla desc limit 100 offset {pageFinal * 10}", conn);
                    var rdr = cmd.ExecuteReader();
                    if (rdr != null && rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            list.Add(Context.Guild.GetUser((ulong)rdr.GetInt64(0)), rdr.GetInt32(1));
                        }
                    } else
                    {
                        await RespondAsync("No users", ephemeral: true);
                        return;
                    }
                    rdr.Close();
                    string message = string.Empty;
                    for (int i = 0; i < Math.Min(10, list.Count); i++)
                    {
                        message += $"**#{i + 1 + (pageFinal * 10)}** | `{list.ElementAt(i).Key.Username}#{list.ElementAt(i).Key.Discriminator}` | **Đ{list.ElementAt(i).Value}**\n";
                    }
                    EmbedBuilder leaderboardEmbed = new EmbedBuilder
                    {
                        Title = $"Leaderboard (Page {pageFinal + 1})",
                        Color = 0x00901A,
                        Description = message
                    };

                    /* ButtonBuilder NextPageButton = new()
                     {
                         Label = "Next Page",
                         CustomId = "next",
                         Style = ButtonStyle.Primary
                     };*/

                    // button functionality comes when i figure out pagination
                    await RespondAsync(embed: leaderboardEmbed.Build());
                }
                catch (Exception ex)
                {
                    await RespondAsync($"```{ex}```", ephemeral: true);

                }

            }
            [SlashCommand("transfer", "Transfer some dolla to someone else")]
            public async Task HandleTransfer(
                [Summary("user", "User to transfer dolla to")] IGuildUser user,
                int amount
                )
            {
                if (amount <= 0)
                {
                    await RespondAsync("Why", ephemeral: true);
                    return;
                }

                if (user == Context.User)
                {
                    await RespondAsync("Why", ephemeral: true);
                    return;
                }

                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();
                    //check if the user is in db.
                    SQLiteCommand cmd = new($"select exists(select UserId from Dolla where UserId={Context.User.Id})", conn);
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        await RespondAsync("You don't have any dolla. How sad.");
                        return;
                    }
                    else
                    {
                        //get amount of dolla
                        SQLiteCommand getDollaCmd = new($"select Dolla from Dolla where UserId={Context.User.Id}", conn);
                        object dollaScal = getDollaCmd.ExecuteScalar();
                        if (dollaScal != null && Convert.ToInt32(dollaScal) > 0)
                        {
                            int senderDolla = Convert.ToInt32(dollaScal);
                            Console.WriteLine("Congrats. you ain't in debt");
                            //Check if reciever is in database
                            SQLiteCommand getRecieverCmd = new($"select exists(select Dolla from Dolla where UserId={user.Id})", conn);
                            object recieverScal = cmd.ExecuteScalar();

                            if (recieverScal != null && Convert.ToInt32(recieverScal) == 0)
                            {
                                SQLiteCommand insertCmd = new($"insert into dolla (UserId, Dolla, LastFreeDolla) values ({user.Id}, {amount}, 0)", conn);
                                insertCmd.ExecuteNonQuery();
                                await RespondAsync($"Successfully transferred Đ{amount} to {user.Username}");
                                SQLiteCommand updateSenderCmd = new($"update Dolla set Dolla={senderDolla - amount} where UserId={Context.User.Id}", conn);
                                updateSenderCmd.ExecuteNonQuery();

                            }
                            else
                            {
                                //get how much dolla
                                SQLiteCommand getRecieverDollaCmd = new($"select Dolla from Dolla where UserId={user.Id}", conn);

                                object recieverDollaScal = getRecieverDollaCmd.ExecuteScalar();

                                if (recieverDollaScal != null)
                                {

                                    int recieverDolla = Convert.ToInt32(recieverDollaScal);

                                    SQLiteCommand updateSenderCmd = new($"update Dolla set Dolla={senderDolla - amount} where UserId={Context.User.Id}", conn);
                                    updateSenderCmd.ExecuteNonQuery();

                                    SQLiteCommand updateRecieverCmd = new($"update Dolla set Dolla={recieverDolla + amount} where UserId={user.Id}", conn);
                                    updateRecieverCmd.ExecuteNonQuery();
                                    await RespondAsync($"Successfully transferred Đ{amount} to {user.Username}");
                                }


                            }
                        }
                        else
                        {
                            await RespondAsync("You do not have enough dolla to transfer dolla to someone else", ephemeral: true);
                            return;
                        }
                    }

                }
                catch (Exception ex) {
                    Console.Write(ex);
                }
            }

            [SlashCommand("balance", "Check your balance")]
            public async Task HandleBalance()
            {

                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new($"select exists(select UserId from Dolla where UserId={Context.User.Id})", conn);
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        // they do not exist in the database, so they have 0 dolla
                        await RespondAsync("You have **Đ0**.", ephemeral: true);
                    } else
                    {
                        SQLiteCommand dollaCmd = new($"select dolla from Dolla where UserId={Context.User.Id}", conn);
                        object dollaScal = dollaCmd.ExecuteScalar();
                        int dolla = Convert.ToInt32(dollaScal);
                        if (scal != null)
                        {
                            await RespondAsync($"You have **Đ{dolla}**.", ephemeral: true);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            [SlashCommand("claim", "Claim your free dolla every 24 hours")]
            public async Task HandleClaim()
            {
                Random random = new();
                string dbPath = @"URI=file:" + config.SQLiteDatabase;
                using var conn = new SQLiteConnection(dbPath);
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new($"select exists(select UserId from Dolla where UserId={Context.User.Id})", conn);
                    object scal = cmd.ExecuteScalar();
                    if (scal != null && Convert.ToInt32(scal) == 0)
                    {
                        int amount = random.Next(1, 16);
                        SQLiteCommand insertCmd = new($"insert into dolla (UserId, Dolla, LastFreeDolla) values ({Context.User.Id}, {amount}, {DateTimeOffset.UtcNow.ToUnixTimeSeconds()})", conn);
                        insertCmd.ExecuteNonQuery();
                        await RespondAsync($"Claimed your free dolla. You now have **Đ{amount}**.");
                    } else
                    {
                        //get amount of dolla and last 
                        SQLiteCommand getInfoCmd = new($"select Dolla,LastFreeDolla from dolla where UserId={Context.User.Id}", conn);
                        var rdr = getInfoCmd.ExecuteReader();
                        int dolla = 0;
                        DateTimeOffset lastFreeDolla = new();
                        if (rdr != null && rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                dolla = rdr.GetInt32(0);
                                lastFreeDolla = DateTimeOffset.FromUnixTimeSeconds(rdr.GetInt32(1));
                            }
                        }
                        rdr.Close();

                        // did they already claim in the last 24 hours?
                        DateTimeOffset now = DateTimeOffset.Now;
                        DateTimeOffset yesterday = now.AddHours(-24);
                       // DateTimeOffset tomorrow = now.AddHours(24);
                        TimeSpan difference = lastFreeDolla.AddHours(24) - now;

                        if (lastFreeDolla > yesterday && lastFreeDolla <= now)
                        {
                            await RespondAsync($"You already claimed your free dolla within the last 24 hours. Wait {difference.Hours} hours, {difference.Minutes} minutes, and {difference.Seconds} seconds. ", ephemeral:true);
                        } else
                        {
                            int amount = random.Next(1, 16);
                            SQLiteCommand updateCmd = new($"update Dolla set Dolla={dolla+amount}, LastFreeDolla={DateTimeOffset.UtcNow.ToUnixTimeSeconds()} where UserId={Context.User.Id}", conn);
                            updateCmd.ExecuteNonQuery();
                            await RespondAsync($"You have claimed your free dolla. You now have **Đ{dolla+ amount}**.", ephemeral: true);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
