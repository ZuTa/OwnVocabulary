using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Data;

namespace OwnVocabulary.Database
{
    public static class DataBase
    {
        private const string STR_CONNECTION = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=..\..\Database\OV.accdb";

        #region Requests to database

        private const string GET_VOCABULARIES = "SELECT * FROM Vocabularies";
        private const string GET_PHRASES = "SELECT Words.ID, Phrase, TranslatedPhrase FROM Words WHERE (Words.VocabularyID=@VocabularyID) AND (Words.Phrase LIKE @Pattern + '%') ORDER BY Phrase";
        private const string CREATE_VOCABULARY = "INSERT INTO Vocabularies(Name, LanguageID) VALUES (@VocabularyName,@LanguageID)";
        private const string GET_VOCABULARY_ID = "SELECT ID FROM Vocabularies WHERE Name=@Name";
        private const string ADD_PHRASE = "INSERT INTO Words (VocabularyID, Phrase, TranslatedPhrase) VALUES (@VocabularyID,@Phrase,@Translate)";
        private const string REMOVE_VOCABULARY = "DELETE FROM Vocabularies WHERE ID=@VocabularyID";
        private const string REMOVE_VOCABULARY_ITEM = "DELETE FROM Words WHERE ID in ({0})";
        
        #endregion

        #region Fields and Properties

        private static OleDbConnection connection;

        private static bool isConnected = false;
        public static bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }
        
        #endregion

        public static void OpenConnection()
        {
            try
            {
                connection = new OleDbConnection(STR_CONNECTION);
                connection.Open();
                isConnected = true;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ErrorCantConnectToDatabase, ex);
            }
        }

        private static void CloseConnection()
        {
            connection.Close();
            isConnected = false;
        }

        public static ObservableCollection<VocabularyItem> GetVocabularies()
        {
            ObservableCollection<VocabularyItem> result = new ObservableCollection<VocabularyItem>();

            try
            {
                OleDbCommand cmd = new OleDbCommand(GET_VOCABULARIES, connection);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new VocabularyItem(int.Parse(reader["ID"].ToString()), reader["Name"].ToString(), int.Parse(reader["LanguageID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception(Properties.Resources.ErrorCantConnectToDatabase, ex);                
            }

            return result;
        }

        public static DataTable GetPhrases(int vocabularyID, string pattern)
        {
            DataTable result = new DataTable();

            using (OleDbCommand cmd = new OleDbCommand(GET_PHRASES, connection))
            {
                cmd.Parameters.AddWithValue("@VocabularyID", vocabularyID);
                cmd.Parameters.AddWithValue("@Pattern", pattern);
                using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(result);
                }
            }
            return result;
        }

        public static VocabularyItem CreateVocabulary(string name, int languageID)
        {
            VocabularyItem item = new VocabularyItem(-1, name, languageID);
            int res = -1;

            using (OleDbCommand cmd = new OleDbCommand(CREATE_VOCABULARY, connection))
            {
                cmd.Parameters.AddWithValue("@VocabularyName", name);
                cmd.Parameters.AddWithValue("@LanguageID", languageID);
                res = cmd.ExecuteNonQuery();
            }
            if (res > 0)
            {
                using (OleDbCommand cmd = new OleDbCommand(GET_VOCABULARY_ID, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    item.ID = int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
            else
                item = null;

            return item;
        }

        public static bool AddPhrase(string phrase, string translate, int vocabularyID)
        {
            bool result;
            int res = 0;

            using( OleDbCommand cmd = new OleDbCommand(ADD_PHRASE,connection))
            {
                cmd.Parameters.AddWithValue("@VocabularyID", vocabularyID);
                cmd.Parameters.AddWithValue("@Phrase", phrase);
                cmd.Parameters.AddWithValue("@Translate", translate);                
                res = cmd.ExecuteNonQuery();
            }
            result = res > 0;

            return result;
        }

        public static bool RemoveVocabulary(int vocabularyID)
        {
            int res = 0;

            using (OleDbCommand cmd = new OleDbCommand(REMOVE_VOCABULARY, connection))
            {
                cmd.Parameters.AddWithValue("@VocabularyID", vocabularyID);
                res = cmd.ExecuteNonQuery();
            }

            return res > 0;
        }

        public static bool RemovePhrases(int[] ids)
        {
            int res = 0;
            string strIDs ="";
            foreach (int id in ids)
            {
                strIDs += id + ",";
            }
            strIDs = strIDs.Remove(strIDs.Length - 1, 1);

            using (OleDbCommand cmd = new OleDbCommand(REMOVE_VOCABULARY_ITEM, connection))
            {
                cmd.CommandText = string.Format(cmd.CommandText, strIDs);
                //cmd.Parameters.AddWithValue("IDs", strIDs);
                res = cmd.ExecuteNonQuery();
            }

            return res > 0;
        }

    }
}
