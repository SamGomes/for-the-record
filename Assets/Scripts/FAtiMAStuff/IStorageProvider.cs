using System.IO;

namespace FAtiMAScripts
{
	public interface IStorageProvider
	{
		Stream LoadFile(string absoluteFilePath, FileMode mode, FileAccess access);
		bool FileExists(string absoluteFilePath);
	}
}