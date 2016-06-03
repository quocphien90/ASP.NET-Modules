using OgilvyOne.Report.DataHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using umbraco.DataLayer;

namespace OgilvyOne.Backend.FileManager
{
	[System.ComponentModel.DataObject]
	public class FileManagerHelper
	{
		private static ISqlHelper SqlHelper
		{
			// trả về đối tượng SqlHelper
			get { return umbraco.BusinessLogic.Application.SqlHelper; }
		}

		[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
		public static List<FileItem> GetListFile(string folderMaster , string path)
		{
			var folderUrl = folderMaster + path;
			if (folderMaster.StartsWith("~") == false)
			{
				folderUrl = "~" + folderUrl;
			}
			var folder = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(folderUrl));
			List<FileItem> kq = new List<FileItem>();
			if (folder.Exists == false)
			{
				return kq;
			}
			//folder
			foreach (var directory in folder.GetDirectories())
			{
				var item = new FileItem()
				{
					Name = directory.Name,
					Size = 0,
					Key = path + directory.Name + "/",
					IsFolder = true,
					FullUrl = (folderUrl + directory.Name).Substring(1)
				};
				item.Thumb = "/umbraco/Images/FileManager/folder.png";
				kq.Add(item);
			}
			//file
			foreach (var file in folder.GetFiles())
			{
				var item = new FileItem()
				{
					Name = file.Name,
					Size = file.Length,
					IsFolder = false,
					Key = path + file.Name,
					FullUrl = (folderUrl + file.Name).Substring(1)

				};
				try
				{
					using (var img = System.Drawing.Image.FromFile(file.FullName))
					{
						item.Thumb = folderUrl + file.Name;
						item.SetWH(img.Width, img.Height);
					}
				}
				catch
				{
					item.Thumb = GetThumb(file.Extension);
				}
				kq.Add(item);
			}
			return kq;
		}

		private static string GetThumb(string Extension)
		{
			return "/umbraco/Images/FileManager/unknown.png";
		}

		public static void Delete(string folderMaster, string fileName)
		{
			if (fileName.EndsWith("/"))
			{
				System.IO.Directory.Delete(HttpContext.Current.Server.MapPath(folderMaster + fileName), true);
			}
			else
			{
				System.IO.File.Delete(HttpContext.Current.Server.MapPath(folderMaster + fileName));
			}
		}

		public static void SaveFile(string folderMaster, string Path, HttpPostedFileBase file)
		{
			var folder =HttpContext.Current.Server.MapPath(folderMaster + Path);
			var fullPath = System.IO.Path.Combine(folder, file.FileName);
			var fileName = file.FileName;
			var fileExt = "";
			var index = file.FileName.LastIndexOf('.');
			if(index != -1)
			{
				fileExt = fileName.Substring(index);
				fileName = fileName.Substring(0, index);
			}
			var sufix = 1;
			while (System.IO.File.Exists(fullPath))
			{
				fullPath = System.IO.Path.Combine(folder, string.Format("{0}-({1}){2}", fileName, sufix, fileExt));
				sufix++;
			}
			file.SaveAs(fullPath);
		}

		public static void NewFolder(string folderMaster, string path, string name)
		{
			var folder = HttpContext.Current.Server.MapPath(folderMaster + path);
			var fullPath = System.IO.Path.Combine(folder , name);
			if (System.IO.Directory.Exists(fullPath) == false)
			{
				System.IO.Directory.CreateDirectory(fullPath);
			}
		}

		public static FileManagerConfig GetFileManagerConfig(long ID)
		{
			using (var reader = SqlHelper.ExecuteReader("SELECT * FROM dsFileManagerConfigs WHERE ([ID] =@ID)", new IParameter[]{
				SqlHelper.CreateParameter("ID", ID)
			}))
			{
				if (reader.Read())
				{
					FileManagerConfig item = new FileManagerConfig();
					DataMapper.Mapper(reader, item);
					return item;
				}
				return null;
			}
		}
		[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
		public static List<FileManagerConfig> GetListFileManagerConfig(FileManagerConfigStatus? status = FileManagerConfigStatus.Active)
		{
			List<FileManagerConfig> kq = new List<FileManagerConfig>();
			using (var reader = SqlHelper.ExecuteReader("SELECT * FROM dsFileManagerConfigs WHERE (@Status IS NULL OR  [Status] =@Status)", new IParameter[]{
				SqlHelper.Parameter("Status", status)
			}))
			{
				while (reader.Read())
				{
					FileManagerConfig item = new FileManagerConfig();
					DataMapper.Mapper(reader, item);
					kq.Add(item);
				}
			}
			return kq;
		}
		public static void InsertFileManagerConfig(FileManagerConfig item)
		{
			if (item.Path.EndsWith("/") == false)
			{
				item.Path = item.Path + "/";
			}
			SqlHelper.ExecuteNonQuery("INSERT INTO dsFileManagerConfigs (Name, Path, Maxrecord, CreatedBy, CreatedDate, Status) VALUES(@Name, @Path, @Maxrecord, @CreatedBy, @CreatedDate, @Status)", new IParameter[]{
				SqlHelper.Parameter("Name", item.Name),
				SqlHelper.Parameter("Path", item.Path),
				SqlHelper.Parameter("Maxrecord",item. Maxrecord),
				SqlHelper.Parameter("CreatedBy", item.CreatedBy),
				SqlHelper.Parameter("CreatedDate", item.CreatedDate),
				SqlHelper.Parameter("Status", item.Status)
			});
		}

		public static void UpdateFileManagerConfig(FileManagerConfig item)
		{
			if (item.Path.EndsWith("/") == false)
			{
				item.Path = item.Path + "/";
			}
			SqlHelper.ExecuteNonQuery("UPDATE dsFileManagerConfigs SET Name=@Name, Path=@Path, Maxrecord=@Maxrecord, CreatedBy=@CreatedBy, CreatedDate=@CreatedDate, Status=@Status WHERE ID = @ID", new IParameter[]{
				SqlHelper.Parameter("ID", item.ID),
				SqlHelper.Parameter("Name", item.Name),
				SqlHelper.Parameter("Path", item.Path),
				SqlHelper.Parameter("Maxrecord",item. Maxrecord),
				SqlHelper.Parameter("CreatedBy", item.CreatedBy),
				SqlHelper.Parameter("CreatedDate", item.CreatedDate),
				SqlHelper.Parameter("Status", item.Status)
			});
		}
		
	}

	public class FileItem
	{
		public const int MaxWH = 125;
		public FileItem()
		{
			Width = Height = 125;
		}

		public string Key { get; set; }
		public string Thumb { get; set; }
		public string Name { get; set; }
		public string FullUrl { get; set; }
		public long Size { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsFolder { get; set; }

		internal void SetWH(int width, int height)
		{
			if (width <= MaxWH && height <= MaxWH)
			{
				this.Width = width;
				this.Height = height;
				return;
			}
			if (width < height)
			{
				this.Height = MaxWH;
				this.Width = (width * this.Height) / height;
				if (this.Width < 1)
				{
					this.Width = 1;
				}
			}
			else
			{
				this.Width = MaxWH;
				this.Height = (height * this.Width) / width;
				if (this.Height < 1)
				{
					this.Height = 1;
				}
			}
		}
	}
	public class  FileManagerConfig
	{
		[Key]
		public long ID { get; set; }
		[MaxLength(250)]
		public string Name { get; set; }
		[MaxLength(4000)]
		public string Path { get; set; }
		public int Maxrecord { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public FileManagerConfigStatus Status { get; set; }
	}
	public enum FileManagerConfigStatus
	{
		UnActive =0,
		Active =1
	}
}

