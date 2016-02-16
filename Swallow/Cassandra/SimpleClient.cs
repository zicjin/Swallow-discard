using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace Swallow.Cassandra {
    public class SimpleClient {
        private static string keyspace = "simplex";
        public Cluster Cluster { get; private set; }
        public ISession Session { get; private set; }

        public virtual void LoadData() { }
        public virtual void QuerySchema() {
            RowSet results = Session.Execute("SELECT * FROM playlists WHERE id = 2cc9ccb7-6221-4ccb-8387-f22b6a1b354d;");
            //Console.WriteLine(String.Format("{0, -30}\t{1, -20}\t{2, -20}\t{3, -30}","title", "album", "artist", "tags"));
            //Console.WriteLine("-------------------------------+-----------------------+--------------------+-------------------------------");
            //foreach (Row row in results.GetRows()) {
            //    Console.WriteLine(String.Format("{0, -30}\t{1, -20}\t{2, -20}\t{3}",
            //        row.GetValue<String>("title"), row.GetValue<String>("album"),
            //        row.GetValue<String>("artist"), row.GetValue<List<String>>("tags").ToString()));
            //}
        }

        public void CreateSchema() {
            Session.Execute("CREATE KEYSPACE " + keyspace + " WITH replication = {'class':'SimpleStrategy', 'replication_factor':3};");
            Session.Execute("CREATE TABLE " + keyspace + ".songs (" +
                "id uuid PRIMARY KEY," +
                "title text," +
                "album text," +
                "artist text," +
                "tags set<text>," +
                "data blob" +
                ");");
            Session.Execute("CREATE TABLE " + keyspace + ".playlists (" +
                "id uuid," +
                "title text," +
                "album text, " +
                "artist text," +
                "song_id uuid," +
                "PRIMARY KEY (id, title, album, artist)" +
                ");");
            Session.Execute("INSERT INTO " + keyspace + ".songs (id, title, album, artist, tags) VALUES (" +
                "756716f7-2e54-4715-9f00-91dcbea6cf50," +
                "'La Petite Tonkinoise'," +
                "'Bye Bye Blackbird'," +
                "'Joséphine Baker'," +
                "{'jazz', '2013'})" +
                ";");
            Session.Execute("INSERT INTO " + keyspace + ".playlists (id, song_id, title, album, artist) VALUES (" +
                "2cc9ccb7-6221-4ccb-8387-f22b6a1b354d," +
                "756716f7-2e54-4715-9f00-91dcbea6cf50," +
                "'La Petite Tonkinoise'," +
                "'Bye Bye Blackbird'," +
                "'Joséphine Baker'" +
                ");");
        }

        public void DropSchema() {
            Session.Execute("DROP KEYSPACE " + keyspace);
            Console.WriteLine("Finished dropping " + keyspace + " keyspace.");
        }

        public void Connect(String node) {
            Cluster = Cluster.Builder()
                   .AddContactPoint(node)
                   .Build();
            //Console.WriteLine("Connected to cluster: " + Cluster.Metadata.ClusterName.ToString());
            //foreach (var host in Cluster.Metadata.AllHosts()) {
            //    Console.WriteLine("Data Center: " + host.Datacenter + ", " +
            //        "Host: " + host.Address + ", " +
            //        "Rack: " + host.Rack);
            //}

            Session = Cluster.Connect();
        }

        public void Close() {
            Cluster.Shutdown();
            Session.Dispose();
        }

        //public static void Main(String[] args) {
        //    SimpleClient client = new SimpleClient();
        //    client.Connect("127.0.0.1");
        //    Console.ReadKey(); // pause the console before exiting
        //    client.Close();
        //}
    }
}
