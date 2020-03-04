using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Converter
{
	[Serializable]
	public class Session
	{
		private static readonly string PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.xml");

		public List<Folder> Folders = new List<Folder>();

		public void Save()
		{
			XmlSerializer xs = new XmlSerializer(typeof(Session));
			using (FileStream fs = new FileStream(PATH, FileMode.OpenOrCreate, FileAccess.Write))
			{
				fs.SetLength(0);
				xs.Serialize(fs, this);
			}
		}

		public static Session Load()
		{
			if (!File.Exists(PATH))
			{
				return new Session();
			}

			XmlSerializer xs = new XmlSerializer(typeof(Session));
			using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
			{
				Session session = (Session)xs.Deserialize(fs);
				return session;
			}
		}

		public DateTime? GetLastTime(string folder)
		{
			foreach (Folder folderObj in this.Folders)
			{
				if (folderObj.Path == folder)
				{
					return folderObj.LastTime;
				}
			}
			return null;
		}

		public void SetLastTime(string folder, DateTime time)
		{
			Folder finded = null;
			foreach (Folder folderObj in this.Folders)
			{
				if (folderObj.Path == folder)
				{
					finded = folderObj;
				}
			}

			if (finded == null)
			{
				finded = new Folder();
				finded.Path = folder;
				this.Folders.Add(finded);
			}

			finded.LastTime = time;
			this.Save();
		}

		public class Folder
		{
			public string Path;
			public DateTime LastTime;
		}
	}
}