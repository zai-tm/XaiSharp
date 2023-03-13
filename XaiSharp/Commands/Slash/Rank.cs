using Discord;
using Discord.Interactions;
using System.Data.SQLite;
using System.Xml.Linq;

namespace XaiSharp.Commands.Slash
{
    public class Rank : InteractionModuleBase<SocketInteractionContext>
    {
        Config config = Util.ParseConfig(File.ReadAllText("config.yml"));

        [EnabledInDm(false)]
        [SlashCommand("rank", "Check your rank")]
        public async Task Handle()
        {
            if (Context.Guild.Id != 990326151987724378)
            {
                await RespondAsync("Sorry but this feature is not available in this server", ephemeral: true);
                return;
            }
            string dbPath = @"URI=file:" + config.SQLiteDatabase;
            using var conn = new SQLiteConnection(dbPath);
            try
            {
                conn.Open();

                SQLiteCommand cmd = new($"select exists(select UserId from ranks where UserId={Context.User.Id})", conn);
                object scal = cmd.ExecuteScalar();
                if (scal != null && Convert.ToInt32(scal) == 0)
                {
                    try
                    {
                        SQLiteCommand insertCmd = new($"insert into ranks (UserId, Rank) values ({Context.User.Id}, 1)", conn);
                        insertCmd.ExecuteNonQuery();
                        await (Context.User as IGuildUser).AddRoleAsync(1083883576728227911);
                        await RespondAsync($"Good job !! You're are level 1 !!!!! You have been given special role");
                    }
                    catch (Exception ex)
                    {
                        await RespondAsync($"```{ex}```", ephemeral: true);
                    }
                }
                else
                {
                    cmd.CommandText = $"select Rank from Ranks where UserId={Context.User.Id}"; // set the command text for checking
                    object rankScal = cmd.ExecuteScalar(); // execute the command and get the result
                    int rank = (int)rankScal;
                    cmd.CommandText = $"update ranks set Rank={rank + 1} where UserId={Context.User.Id}"; // set the command text for updating
                    cmd.ExecuteNonQuery(); // execute the command and update the database
                    await RespondAsync($"Good job !! You're are level {rank + 1} !!!!! Sadly oyu already have special role :(");
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
