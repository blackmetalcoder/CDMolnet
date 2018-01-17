using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using ParkSquare.Gracenote;
using Formatting = Newtonsoft.Json.Formatting;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;


namespace WebDemoCDMolnet
{
    /// <summary>
    /// Summary description for CDService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class CDService : System.Web.Services.WebService
    {
        public string sConn = "Data Source=arsenal-135426.mssql.binero.se;database=135426-arsenal;uid=135426_lb11397;pwd=Phantom16540000@";
        [WebMethod]
        public string Decrypt_pwd(string p)
        {
            string svar = string.Empty;
            string hash = "kfg9€$pp==88!?";
            byte[] data = Convert.FromBase64String(p);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    svar = UTF8Encoding.UTF8.GetString(results);
                }
            }
                return svar;
        }
        [WebMethod]
        public string Encrypt_pwd(string p)
        {
            string svar = string.Empty;
            string hash = "kfg9€$pp==88!?";
            byte[] data = UTF8Encoding.UTF8.GetBytes(p);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    svar = Convert.ToBase64String(results, 0, results.Length);
                }
            }
            return svar;
        }
        [WebMethod]
        public string HelloWorld(string input)
        {
            return input;
        }
        [WebMethod(Description = "Kollar om användarnamn är upptaget")]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string finnsUser(string userNamn)
        {
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT * FROM TbUser WHERE Username = '" + userNamn + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        
                        if (reader.HasRows)
                        {
                            return "true";
                        }
                        else return "false";
                    }
                    catch (Exception ex)
                    {
                        return "true";
                    }
                    cmd.Connection.Close();
                }
            }
        }
        [WebMethod(Description = "Skapar ny användare till CDMOLNET")]
        public string addUserEnc(string fnamn, string enamn, string epost, string usernamn, string password)
        {
            string sfnamn = Decrypt_pwd(fnamn);
            string sepost = Decrypt_pwd(epost);
            string susernamn = Decrypt_pwd(usernamn);
            string spassword = Decrypt_pwd(password);
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertUser";
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        cmd.Parameters.Add("@fnamn", SqlDbType.NChar).Value = sfnamn;
                        cmd.Parameters.Add("@enamn", SqlDbType.NChar).Value = enamn;
                        cmd.Parameters.Add("@epost", SqlDbType.NChar).Value = sepost;
                        cmd.Parameters.Add("@usernamn", SqlDbType.NChar).Value = susernamn;
                        cmd.Parameters.Add("@password", SqlDbType.NChar).Value = spassword;
                        cmd.ExecuteNonQuery();
                        return susernamn + ", your are new MyRecords user";
                    }
                    catch (Exception ex)
                    {
                        return "Something went wrong, try again!";
                    }
                }
            }
        }
        [WebMethod(Description = "Skapar ny användare till CDMOLNET")]
        public string addUser(string fnamn, string enamn, string epost, string usernamn, string password)
        {
            
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertUser";
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        cmd.Parameters.Add("@fnamn", SqlDbType.NChar).Value = fnamn;
                        cmd.Parameters.Add("@enamn", SqlDbType.NChar).Value = enamn;
                        cmd.Parameters.Add("@epost", SqlDbType.NChar).Value = epost;
                        cmd.Parameters.Add("@usernamn", SqlDbType.NChar).Value = usernamn;
                        cmd.Parameters.Add("@password", SqlDbType.NChar).Value = password;
                        cmd.ExecuteNonQuery();
                        return usernamn + ", your are new MyRecords user";
                    }
                    catch (Exception ex)
                    {
                        return "Something wenty wrong, try again!";
                    }
                }
            }
        }
        [WebMethod(Description = "Loggar in användare")]
        public string loggaIn(string usernamn, string password)
        {
            string svar = string.Empty; ;
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM TbUser WHERE Username= '" + usernamn + "' And password = '" + password + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            svar = reader["Fnamn"].ToString().Trim() + " " + reader["Enamn"].ToString().Trim() + ";" + reader["id"].ToString().Trim();
                        }
                    }
                    else
                    {
                        svar = "NO";
                    }
                    cmd.Connection.Close();
                }
            }
            return svar;
        }
        [WebMethod(Description ="Loggar in användare encrypted")]
        public string EncLoggin(string usernamn, string password)
        {
            string svar = string.Empty;
            string user = Decrypt_pwd(usernamn);
            string pwd = Decrypt_pwd(password);
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM TbUser WHERE Username= '" + user + "' And password = '" + pwd + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            svar = reader["Fnamn"].ToString().Trim() + " " + reader["Enamn"].ToString().Trim() + ";" + reader["id"].ToString().Trim();
                        }
                    }
                    else
                    {
                        svar = "NO";
                    }
                    cmd.Connection.Close();
                }
            }
            svar = Encrypt_pwd(svar);
            return svar;
        }
        [WebMethod(Description = "Hämtar artister i ordning")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getArtist10(string userID)
        {
            string svar = string.Empty;
            List<Artist> A = new List<Artist>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vArtister WHERE UserId = " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Artist a = new Artist();
                        a.artist = reader["Artist"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar artister i ordning")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getArtistElectron(int userID)
        {
            string svar = string.Empty;
            List<Artist> A = new List<Artist>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vArtister WHERE UserId = " + userID.ToString();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Artist a = new Artist();
                        a.artist = reader["Artist"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar Album per artist/grupp")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbum10(string userID, string Artist)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE Artist = '" + Artist + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.ID = int.Parse(reader["id"].ToString().Trim());
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json; //new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod(Description = "Hämtar artister i ordning")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object getArtist(string userID)
        {
            string svar = string.Empty;
            List<Artist> A = new List<Artist>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vArtister WHERE UserId = " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Artist a = new Artist();
                        a.artist = reader["Artist"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return new JavaScriptSerializer().DeserializeObject(json);
        }
       /* [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Hämtar de 50 senaste innlagda")]
        public object senasteInnlagda(int userId)
        {
            List<SenasteAlbum> A = new List<SenasteAlbum>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT TOP (30) Artist, Album, bild, id, UserId, media " +
                                      "FROM dbo.tbAlbum " +
                                      "WHERE (UserId = " + userId.ToString() + ") " +
                                      "ORDER BY id DESC";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SenasteAlbum a = new SenasteAlbum();
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.Album = reader["Album"].ToString().Trim();
                        a.Bild = reader["bild"].ToString().Trim();
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(A, Formatting.Indented);
            return new JavaScriptSerializer().DeserializeObject(json);
        }*/
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Användar statistik")]
        public string minStat(string userId)
        {
            string json = string.Empty;
            List<CDinfo> CD = new List<CDinfo>();
          
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT COUNT(DISTINCT Artist) AS Artister, COUNT(Album) AS Album, COUNT(DISTINCT kategori) AS Kategorier " +
                                      "FROM dbo.tbAlbum " +
                                      "WHERE (UserId = " + userId + ")";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CDinfo cd = new CDinfo();
                        cd.Text = "Artists/Groups";
                        cd.Antal = int.Parse(reader["Artister"].ToString());
                        CD.Add(cd);
                        CDinfo cd2 = new CDinfo();
                        cd2.Text = "Albums";
                        cd2.Antal = int.Parse(reader["Album"].ToString());
                        CD.Add(cd2);
                        /*CDinfo cd3 = new CDinfo();
                        cd3.Text = "Kategorier";
                        cd3.Antal = int.Parse(reader["Kategorier"].ToString());
                        CD.Add(cd3);*/
                    }
                }
            }
            using (SqlConnection con2 = new SqlConnection(sConn))
            {
                using (SqlCommand cmd2 = new SqlCommand())
                {
                    cmd2.CommandText = "SELECT COUNT(DISTINCT id) AS Tracks " + 
                                       "FROM dbo.tbTracks " + 
                                       "WHERE (UserId = " + userId + ")";
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Connection = con2;
                    cmd2.Connection.Open();
                    SqlDataReader reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        CDinfo cd = new CDinfo();
                        cd.Text = "Tracks";
                        cd.Antal = int.Parse(reader2["Tracks"].ToString());
                        CD.Add(cd);
                    }
                }
            }
            //CD.Add(cd);
            json = JsonConvert.SerializeObject(CD, Formatting.Indented);
            return json;
            //return new JavaScriptSerializer().DeserializeObject(json);
        }
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        [WebMethod (Description="Antal cd per år för graph")]
        public string AntalperAr(string  userId)
        {
            List<CDperAr> C = new List<CDperAr>();
            string sSQL = "Select tbAlbum.Ar, Count(tbAlbum.Ar) As Antal, tbAlbum.UserId " +
                          "From tbAlbum " +
                          "Group By tbAlbum.Ar, tbAlbum.UserId " +
                          "Having tbAlbum.UserId = " + userId +
                          "Order By tbAlbum.Ar Desc";
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CDperAr c = new CDperAr();
                        c.Ar = reader["Ar"].ToString();
                        c.antal = int.Parse(reader["Antal"].ToString());
                        C.Add(c);
                    }
                }
            }
           string json = JsonConvert.SerializeObject(C, Formatting.Indented);
            return json;
        }
        [WebMethod(Description = "Antal per media till PIE-graph till PHP-app")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string mediaPHP(string userId)
        {
            string sReturn;
            List<PieMedia> P = new List<PieMedia>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spAntalPerMedia";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = int.Parse(userId.ToString());
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    sReturn = "'{cols': [ " +
                                              "{'id':'','label':'Media','pattern':'','type':'string'}," +
                                              "{'id':'','label':'Slices','pattern':'','type':'number'}" +
                                             "]," +
                                     "'rows': [ ";
                    while (reader.Read())
                    {
                        sReturn += "{ 'c':[{'v':" + reader["Media"].ToString() + ",'':null},{'v':" + reader["Antal"].ToString() + ",'f':null}]},";
                        /*PieMedia p = new PieMedia();
                        p.media = reader["Media"].ToString();
                        p.varde = int.Parse(reader["Antal"].ToString());
                        P.Add(p);*/
                    }
                    sReturn += "]}";
                }
            }
            //string json = JsonConvert.SerializeObject(P, Formatting.Indented);
            return sReturn;
        }
        [WebMethod(Description = "Antal per media till PIE-graph till PHP-app")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string media(string userId)
        {
            List<PieMedia> P = new List<PieMedia>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "spAntalPerMedia";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = int.Parse(userId.ToString());
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PieMedia p = new PieMedia();
                        p.media = reader["Media"].ToString();
                        p.varde = int.Parse(reader["Antal"].ToString());
                        P.Add(p);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(P, Formatting.Indented);
            return json;
        }
        [WebMethod(Description = "Hämtar Album per artist/grupp")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object getAlbum(string userID, string Artist)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE Artist = '" + Artist + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod(Description = "Hämtar Tracks för valt album")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object getTracks(string userID, string DiscID)
        {
            string svar = string.Empty;
            List<Tracks> A = new List<Tracks>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbTracks WHERE discid = '" + DiscID + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tracks a = new Tracks();
                        a.track = reader["Track"].ToString().Trim();
                        a.nr = int.Parse(reader["nr"].ToString().Trim());
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod(Description = "Hämtar artist/grupp info")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string artistInfo(string artist)
        {
            string svar = string.Empty;
            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            var sBild = string.Empty;
            var sInfo = string.Empty;
            System.Xml.XmlDocument dokument = new System.Xml.XmlDocument();
            dokument.Load(bildQuery);
            sBild = dokument.GetElementsByTagName("image")[3].InnerText;
            sInfo = dokument.GetElementsByTagName("content")[0].InnerText;
            svar = "<Table><tr><td align=center><img src='" + sBild + "'/></td></tr>";
            svar += "<tr><td>" + sInfo + "/></td></tr></table>"; 
            return svar;
        }
        [WebMethod(Description = "Hämtar top album för vald grupp/artist")]
        public string topAlbum(string artist)
        {
            string svar = string.Empty;
            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.gettopalbums&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            var sBild = string.Empty;
            var sInfo = string.Empty;
            System.Xml.XmlDocument dokument2 = new System.Xml.XmlDocument();
            dokument2.Load(bildQuery);
            var i = dokument2.GetElementsByTagName("name").Count;
            svar += "<ul class='list-group'>";
            for (int j = 0; j != i; j = j +2)
                {
                    svar += "<li class='list-group-item'>" + dokument2.GetElementsByTagName("name")[j].InnerText + "</li>";
                    //svar += dokument2.GetElementsByTagName("name")[j].InnerText + "<br>";
                }
            svar += "</ul>";
            return svar;
        }
        [WebMethod(Description = "Hämta liknande artister/grupper")]
        public string artistLiknande(string artist)
        {
            string svar = string.Empty;

            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getsimilar&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            var sBild = string.Empty;
            var sInfo = string.Empty;
            System.Xml.Linq.XDocument dokument = System.Xml.Linq.XDocument.Load(bildQuery);
            var grupper = dokument.Descendants("name");
            svar += "<ul class='list-group'>";
            foreach ( var grupp in grupper )
            {
                svar += "<li class='list-group-item'>" + grupp.Value + "</li>";
            }
            svar += "</ul>";
            return svar;
        }
        [WebMethod(Description = "Hämtar Tracks för vald artist")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object getTracksByArtist(string userID, string Artist)
        {
            string svar = string.Empty;
            List<Tracks> A = new List<Tracks>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM [vTracksArtist] WHERE Artist = '" + Artist + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tracks a = new Tracks();
                        a.track = reader["Track"].ToString().Trim();
                        a.nr = int.Parse(reader["nr"].ToString().Trim());
                        a.discID = reader["discid"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object ArtistInfo(string artist)
        {
            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            XmlDocument dokument = new XmlDocument();
            dokument.Load(bildQuery);
            /**/
            List<cArtist> A = new List<cArtist>();
            cArtist a = new cArtist();
            a.artist = artist;
            a.artistBild = dokument.GetElementsByTagName("image")[4].InnerText;
            a.InfoArtist = dokument.GetElementsByTagName("content")[0].InnerText;
            A.Add(a);
            string sJson = JsonConvert.SerializeObject(A, Newtonsoft.Json.Formatting.Indented);
            return new JavaScriptSerializer().DeserializeObject(sJson);
        }
        public static string MinifyA(string p)
        {
            p = p.Replace(" ", string.Empty);
            return p;
        }
        public static string MinifyBio(string p)
        {           
            p = p.Replace("\n", string.Empty);
            p = p.Replace("</p>", "\n\n");
            p = p.Replace("<p>", string.Empty);
            p = p.Replace("&quot;", "'");
            p = p.Replace("&#039;", string.Empty);
            return p;
        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object LiknandeArtister(string artist)
        {
            string BildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getsimilar&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            XmlDocument dokument = new XmlDocument();
            dokument.Load(BildQuery);
            XDocument loadedData1 = XDocument.Parse(dokument.InnerXml);
            XmlDocument dokument1 = new XmlDocument();
            var data = from query in loadedData1.Document.Descendants("artist")
                       select new LikandeArtister()
                       {
                           namn = (string)query.Element("name"),
                           bild = (string)query.Element("image"),
                           infoURL = (string)query.Element("url"),
                           info = " ",
                       };
            List<LikandeArtister> L = data.ToList<LikandeArtister>();
            foreach (var aa in L)
            {
                try
                {
                    string BildQ = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + aa.namn + "&api_key=c5e414764357baf4097c59e86df16154";
                    XmlDocument dokument2 = new XmlDocument();
                    dokument2.Load(BildQ);
                    aa.bild = dokument2.GetElementsByTagName("image")[4].InnerText;
                    aa.info = dokument2.GetElementsByTagName("summary")[0].InnerText;

                }
                catch (Exception ex1)
                {
                    aa.bild = "";
                }
            }
            string sJson = JsonConvert.SerializeObject(L, Newtonsoft.Json.Formatting.Indented);
            return new JavaScriptSerializer().DeserializeObject(sJson);
        }
        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object ShortAristInfo(string artist)
        {
            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            XmlDocument dokument = new XmlDocument();
            dokument.Load(bildQuery);
            /**/
            List<cArtist> A = new List<cArtist>();
            cArtist a = new cArtist();
            a.artist = artist;
            a.artistBild = dokument.GetElementsByTagName("image")[4].InnerText;
            a.InfoArtist = dokument.GetElementsByTagName("content")[0].InnerText;
            A.Add(a);
            string sJson = JsonConvert.SerializeObject(A, Newtonsoft.Json.Formatting.Indented);
            return new JavaScriptSerializer().DeserializeObject(sJson);
        }    
        [WebMethod(Description = "Hämtar Artist/Album från GraceNote")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string GetArtistAlbum(String Artist)
        {
            string svar = string.Empty;
            var G = SkivaDataSource.GetInfo(Artist);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(G);
            return json;
        }
        [WebMethod(Description = "Hämtar Album från GraceNote")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string AlbumTracks(string Artist, string Album)
        {
            string svar = string.Empty;
            var G = SkivaDataSource.GetAlbum(Artist, Album);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(G);
            return json;
        }
        [WebMethod(Description = "Hämtar Album från GraceNote PHP")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public object AlbumTracksPHP(string Artist, string Album)
        {
            string svar = string.Empty;
            var G = SkivaDataSource.GetAlbum(Artist, Album);
            string sJson = JsonConvert.SerializeObject(G, Formatting.Indented);
            return new JavaScriptSerializer().DeserializeObject(sJson);
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(G);
            //return json;
        }
        [WebMethod(Description = "Sparar Album från GraceNote")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string sparaAlbum(string Artist, string Album, int Ar, string kategori, string discid, int userID, string bild, string media, int manuell)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "spInsertAlbum";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@discid", SqlDbType.NChar).Value = discid;
                        cmd.Parameters.Add("@Album", SqlDbType.NChar).Value = Album;
                        cmd.Parameters.Add("@Ar", SqlDbType.Int).Value = Ar;
                        cmd.Parameters.Add("@kategori", SqlDbType.NChar).Value = kategori;
                        cmd.Parameters.Add("@Artist", SqlDbType.NChar).Value = Artist;
                        cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                        cmd.Parameters.Add("@Bild", SqlDbType.NChar).Value = bild;
                        cmd.Parameters.Add("@Media", SqlDbType.NChar).Value = media;
                        cmd.Parameters.Add("@Manuell", SqlDbType.Int).Value = manuell;
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                return "Sparat";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        [WebMethod(Description = "Sparar Tracks från GraceNote")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string sparaTrack(int no, string lat, string discid, int userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "spInsertTracks";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@discid", SqlDbType.NChar).Value = discid;
                        cmd.Parameters.Add("@Track", SqlDbType.NChar).Value = lat;
                        cmd.Parameters.Add("@nr", SqlDbType.Int).Value = no;
                        cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return "Sparat";
        }
        [WebMethod(Description = "Kollar så att platta inte redan finns")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public bool finnsAlbum(string discid, int userID)
        {
            bool finns = false;
            try
            {
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "SELECT * FROM tbAlbum WHERE discid = '" + discid.Trim() + "' AND UserId = " + userID;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            finns = true;
                        }
                        cmd.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //return ex.Message.ToString();
            }
            return finns;
        }
        [WebMethod(Description = "Hämtar Tracks för valt album för Win 10 App")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getTracks10(string userID, string DiscID)
        {
            string svar = string.Empty;
            List<Tracks> A = new List<Tracks>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbTracks WHERE discid = '" + DiscID + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tracks a = new Tracks();
                        a.track = reader["Track"].ToString().Trim();
                        a.nr = int.Parse(reader["nr"].ToString().Trim());
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar CD per artist/grupp för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbumCD10(string userID, string Artist)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE Media = 'CD' And Artist = '" + Artist + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json; //new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod(Description = "Hämtar CD per artist/grupp för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbumVinyl10(string userID, string Artist)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE Media = 'Vinyl' And Artist = '" + Artist + "' AND userId= " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json; //new JavaScriptSerializer().DeserializeObject(json);
        }
        [WebMethod(Description = "Hämtar artister på Vinyl i ordning för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getArtistVinyl10(string userID)
        {
            string svar = string.Empty;
            List<ArtistVinyl> A = new List<ArtistVinyl>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vListaVinyl WHERE UserId = " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ArtistVinyl a = new ArtistVinyl();
                        a.artistVinyl = reader["Artist"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar artister på CD i ordning för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getArtistCD10(string userID)
        {
            string svar = string.Empty;
            List<ArtistCD> A = new List<ArtistCD>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vListaCD WHERE UserId = " + userID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ArtistCD a = new ArtistCD();
                        a.artistCD = reader["Artist"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Uppdaterar album för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string UppdateraAlbum(string Artist, string Album, int Ar, string kategori, string bild, string media, int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "spUpdateAlbum";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Album", SqlDbType.NChar).Value = Album;
                        cmd.Parameters.Add("@Ar", SqlDbType.Int).Value = Ar;
                        cmd.Parameters.Add("@kategori", SqlDbType.NChar).Value = kategori;
                        cmd.Parameters.Add("@Artist", SqlDbType.NChar).Value = Artist;
                        cmd.Parameters.Add("@bild", SqlDbType.NChar).Value = bild;
                        cmd.Parameters.Add("@Media", SqlDbType.NChar).Value = media;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                return "Sparat";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        [WebMethod(Description = "Hämtar Album per artist/grupp")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbumByUser10(string userID)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE userId= " + userID + " Order By Artist,Album";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.ID = int.Parse(reader["id"].ToString().Trim());
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar Album per artist/grupp för Windows 10")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbumByID10(string ID)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM tbAlbum WHERE id= " + ID;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.ID = int.Parse(reader["id"].ToString().Trim());
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Hämtar de 50 senaste innlagda för Windows 10")]
        public string senasteInnlagda10(int userId)
        {
            List<SenasteAlbum> A = new List<SenasteAlbum>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT TOP (50) * " +
                                      "FROM dbo.tbAlbum " +
                                      "WHERE (UserId = " + userId.ToString() + ") " +
                                      "ORDER BY id DESC";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SenasteAlbum a = new SenasteAlbum();
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.album = reader["Album"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.Media = reader["media"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.ID = int.Parse(reader["id"].ToString().Trim());
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Kategori = reader["Kategori"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(A, Formatting.Indented);
            return json;
        }
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Album per år för Windows 10")]
        public string ByYear10(int userId)
        {
            List<PerAr> A = new List<PerAr>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * " +
                                      "FROM dbo.vAr " +
                                      "WHERE UserId = " + userId.ToString() + " Order By Ar Desc";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PerAr a = new PerAr();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                       
                        A.Add(a);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(A, Formatting.Indented);
            return json;
        }
        [WebMethod(Description = "Hämtar Album per artist/grupp")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string getAlbumByYear10(string userID, int Ar)
        {
            string svar = string.Empty;
            List<Album> A = new List<Album>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vArtistAr WHERE userId= " + userID + " And Ar = " + Ar;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Album a = new Album();
                        a.ID = int.Parse(reader["id"].ToString().Trim());
                        a.Artist = reader["Artist"].ToString().Trim();
                        a.album = reader["Album"].ToString().Trim();
                        a.Kategori = reader["kategori"].ToString().Trim();
                        a.Cover = reader["bild"].ToString().Trim();
                        a.discID = reader["discid"].ToString().Trim();
                        a.Ar = int.Parse(reader["Ar"].ToString().Trim());
                        a.Media = reader["media"].ToString().Trim();
                        A.Add(a);
                    }
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
            return json;
        }
        [WebMethod(Description = "Hämtar ID för mauellt inlagda skivor")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public int geID(int user)
        {
            int svar = 0;
            try
            {
                string sSQL = "SELECT TOP (100) PERCENT id, discid, Artist, Album, manuell, UserId " +
                              "FROM dbo.tbAlbum " +
                              "WHERE(manuell = 1) AND(UserId = 1) " +
                              "ORDER BY id";
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = sSQL;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int i = int.Parse(reader["discid"].ToString());
                            i++;
                            svar = i;
                        }
                        cmd.Connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                //return ex.Message.ToString();
            }
            return svar;
        }
        [WebMethod(Description = "Random spel-lista")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string RandomAlbum(int User, string Media, int antal)
        {
            string svar = string.Empty;
            List<RandomAlbums> R = new List<RandomAlbums>();
            try
            {
                using (SqlConnection con = new SqlConnection(sConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "spRandomPlaylist";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = User;
                        cmd.Parameters.Add("@Media", SqlDbType.NChar).Value = Media + "%";
                        cmd.Connection = con;
                        cmd.Connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                       
                        while (reader.Read())
                        {
                            RandomAlbums r = new RandomAlbums();

                            r.Id = int.Parse(reader["id"].ToString());
                            R.Add(r);
                        }
                        cmd.Connection.Close();

                    }
                }
                var rnd = new Random();
                var LL = R.OrderBy(x => rnd.Next()).Take(antal);
                //Nu hämtar jag albumen
                List<Album> A = new List<Album>();
                foreach(var item in LL)
                {
                    Album a = new Album();
                    var aa = getAlbumForPlaylist(item.Id);
                    a.album = aa.album;
                    a.Artist = aa.Artist;
                    a.Cover = aa.Cover;
                    a.Media = aa.Media;
                    A.Add(aa);
                }
                //Slut hämta album
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(A);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
           
        }
        [WebMethod(Description = "Random spel-lista")]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public Album getAlbumForPlaylist(int id)
        {
            string sSQL = "SELECT * FROM tbAlbum WHERE id=" + id.ToString();
            Album l = new Album();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand(sSQL, con))
                {
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        l.Artist = reader["Artist"].ToString().Trim();
                        l.album = reader["Album"].ToString().Trim();
                        l.Media = reader["Media"].ToString().Trim();
                        l.Cover = reader["bild"].ToString().Trim();
                    }
                    cmd.Connection.Close();
                }
            }
            return l;
        }
        [WebMethod(Description = "Sparar album från Mobil-PHP App")]
        public string SparaAlbumPHP(string albumID, string userID, string media)
        {
            string Svar = string.Empty;
            try
            {
                var client = new GracenoteClient("687559541-407A6818314DF26BB17DC1E4AB57BD4E");
                SearchResult result;
                result = client.Search("99337157-E1ED6101A3666CFD7D799529ED707E0F");
                foreach (var skiva in result.Albums)
                {
                    string s = "";
                    try
                    {
                        s = result.Albums.First().Artwork.First().Uri.AbsoluteUri;
                    }
                    catch (Exception ex)
                    {
                        s = "";
                    }
                    //Sparar Album med userID

                    //Sparar tracks med userID & albumid
                    using (SqlConnection con = new SqlConnection(sConn))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {

                            cmd.CommandText = "spInsertAlbum";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@discid", SqlDbType.NChar).Value = skiva.Id;
                            cmd.Parameters.Add("@Album", SqlDbType.NChar).Value = skiva.Title;
                            cmd.Parameters.Add("@Ar", SqlDbType.Int).Value = skiva.Year.ToString();
                            cmd.Parameters.Add("@kategori", SqlDbType.NChar).Value = skiva.Genre.First();
                            cmd.Parameters.Add("@Artist", SqlDbType.NChar).Value = skiva.Artist;
                            cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                            cmd.Parameters.Add("@Bild", SqlDbType.NChar).Value = s;
                            cmd.Parameters.Add("@Media", SqlDbType.NChar).Value = media;
                            cmd.Parameters.Add("@Manuell", SqlDbType.Int).Value = 0;
                            cmd.Connection = con;
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                    }
                    Skiva NySkiva = new Skiva(skiva.Id, skiva.Title, skiva.Year.ToString(), s, skiva.Genre.First());
                    foreach (var track in skiva.Tracks)
                    {
                        using (SqlConnection con = new SqlConnection(sConn))
                        {
                            using (SqlCommand cmd = new SqlCommand())
                            {

                                cmd.CommandText = "spInsertTracks";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@discid", SqlDbType.NChar).Value = track.Id;
                                cmd.Parameters.Add("@Track", SqlDbType.NChar).Value = track.Title;
                                cmd.Parameters.Add("@nr", SqlDbType.Int).Value = track.Number.ToString();
                                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = int.Parse(userID);
                                cmd.Connection = con;
                                cmd.Connection.Open();
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                        }
                        //NySkiva.Lat.Add(new Latar(track.Number.ToString(), track.Title, track.Id));
                    }
                }
            }
            catch (Exception ex)
            {
                Svar = "Something went wrong!";
            }
            return Svar;
        }
        [WebMethod]
        public string AristInfoTEN(string artist)
        {
            try
            {
                string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
                XmlDocument dokument = new XmlDocument();
                dokument.Load(bildQuery);
                /**/
                List<cArtist> A = new List<cArtist>();
                cArtist a = new cArtist();
                a.artist = artist;
                a.artistBild = dokument.GetElementsByTagName("image")[4].InnerText;
                a.InfoArtist = dokument.GetElementsByTagName("content")[0].InnerText;
                A.Add(a);
                string sJson = JsonConvert.SerializeObject(A, Newtonsoft.Json.Formatting.Indented);
                return sJson;
            }
            catch (Exception ex)
            {
                return "FEL";
            }
        }
        [WebMethod]
        public string LiknandeArtister10(string artist)
        {
            string BildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.getsimilar&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            XmlDocument dokument = new XmlDocument();
            dokument.Load(BildQuery);
            XDocument loadedData1 = XDocument.Parse(dokument.InnerXml);
            XmlDocument dokument1 = new XmlDocument();
            var data = from query in loadedData1.Document.Descendants("artist")
                       select new LikandeArtister()
                       {
                           namn = (string)query.Element("name"),
                           bild = (string)query.Element("image"),
                           infoURL = (string)query.Element("url"),
                           info = " ",
                       };
            List<LikandeArtister> L = data.ToList<LikandeArtister>();
           /* foreach (var aa in L)
            {
                try
                {
                    string BildQ = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + aa.namn + "&api_key=c5e414764357baf4097c59e86df16154";
                    XmlDocument dokument2 = new XmlDocument();
                    dokument2.Load(BildQ);
                    aa.bild = dokument2.GetElementsByTagName("image")[4].InnerText;
                    aa.info = dokument2.GetElementsByTagName("summary")[0].InnerText;

                }
                catch (Exception ex1)
                {
                    aa.bild = "";
                }
            }*/
            string sJson = JsonConvert.SerializeObject(L, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }
        [WebMethod]
        public string topAlbum10(string artist)
        {
            string svar = string.Empty;
            string bildQuery = "http://ws.audioscrobbler.com/2.0/?method=artist.gettopalbums&artist=" + artist + "&api_key=c5e414764357baf4097c59e86df16154";
            var sBild = string.Empty;
            var sInfo = string.Empty;
            System.Xml.XmlDocument dokument = new System.Xml.XmlDocument();
            dokument.Load(bildQuery);
            XDocument loadedData1 = XDocument.Parse(dokument.InnerXml);
            XmlDocument dokument1 = new XmlDocument();
            List<topAlbums> listAlbum = (
                from pur in loadedData1.Descendants("album").Descendants("artist")
                select new topAlbums()
                {
                    Album = (string)pur.Parent.Element("name"),
                    Artist = (string)pur.Element("name"),
                    AlbunID = (string)pur.Element("mbid"),
                    Coverart = (string)pur.Parent.Element("image"),
                }).ToList();
            string sJson = JsonConvert.SerializeObject(listAlbum, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }
        [WebMethod]
        public string getBilder(string Q)
        {
            string svar = string.Empty;

            System.Net.WebClient wc = new System.Net.WebClient();
            List<Bilder> B = new List<Bilder>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
             string address = "https://www.bing.com/images/search?q=" + Q + "&qs=n&form=QBLH&scope=images&sp=-1&pq=" + Q + "&sc=8-6&sk=&cvid=054C4C12E3384CEB8ACC9E3BA680C6DD";
            //string address = "https://www.google.com/search?q=" + Q + "&tbm=isch&gws_rd=cr&ei=16E0WMGSKYmisAHmp6b4Ag";
            //string address = "https://www.google.com/search?q=" + Q + "&tbm=isch&gws_rd=cr&ei=16E0WMGSKYmisAHmp6b4Ag";
            doc.Load(wc.OpenRead(address)); 
            HtmlNodeCollection imgs = doc.DocumentNode.SelectNodes("//img[@src]");
            foreach (HtmlNode img in imgs)
            {
                if (img.Attributes["src"] == null)
                    continue;
                HtmlAttribute src = img.Attributes["src"];
                Bilder b = new Bilder();
                b.kalla = src.Value;
                B.Add(b);

            }
            string sJson = JsonConvert.SerializeObject(B, Newtonsoft.Json.Formatting.Indented);
            return sJson;
        }
    }
    

}
