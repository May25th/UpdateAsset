using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SevenZip.Compression.LZMA;
using System.IO;
using System;

public class TestLZMABigFile:Editor 
{
    [MenuItem ("MyMenu/CompressBigFile")]
	static void CompressFile () 
	{
		//压缩文件
		//Application.dataPath 编辑器环境下是 Asset文件夹目录
		//CompressFileLZMA(Application.dataPath+"/1.jpg",Application.dataPath+"/2.zip");
		CompressFileLZMA(Application.dataPath+"/1.jpg",Application.dataPath+"/test.jpg");
		AssetDatabase.Refresh();
 
	}
	[MenuItem ("MyMenu/DecompressBigFile")]
	static void DecompressFile () 
	{
		//解压文件
		DecompressFileLZMA(Application.dataPath+"/2.zip",Application.dataPath+"/3.jpg");
		AssetDatabase.Refresh();
	}

	[MenuItem ("MyMenu/XXXXX")]
	static void XXXXX () 
	{
		//解压文件
		//CopyFile(Application.dataPath+"/1.jpg",Application.dataPath+"/3.jpg", 64);
		Test(Application.dataPath+"/1.jpg",Application.dataPath+"/fsWriter.zip");
		AssetDatabase.Refresh();
	}

	private static void Test(string fromPath, string toPath)
	{
		//创建读取文件的流
		using (FileStream fsReader = new FileStream(fromPath, FileMode.Open))
		{
			//创建写入文件的流
			using (FileStream fsWriter = new FileStream(toPath, FileMode.OpenOrCreate))
			{
				SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
				coder.WriteCoderProperties(fsWriter);
				//创建一个5M的缓冲区
				byte[] buffers = new byte[1024 * 1024 * 5];
				int i = 0;
				//文件读取到缓冲区
				while ((i = fsReader.Read(buffers, 0, buffers.Length)) > 0)
				{
					//将缓冲区中内容写入fsWriter流
					fsWriter.Write(buffers, 0, i);
					//显示进度
					long l = fsWriter.Length;
					double proc = (double)l / fsReader.Length;
					Debug.LogFormat("拷贝进度{0}%", proc * 100);
				}
				coder.Code(fsReader, fsWriter, fsReader.Length, -1, null);//
			}
		}
	}
	
	/// <summary>
	/// 复制大文件
	/// </summary>
	/// <param name="fromPath">源文件的路径</param>
	/// <param name="toPath">文件保存的路径</param>
	/// <param name="eachReadLength">每次读取的长度</param>
	/// <returns>是否复制成功</returns>
	public static bool CopyFile(string fromPath, string toPath, int eachReadLength)
	{
		//将源文件 读取成文件流
		FileStream fromFile = new FileStream(fromPath, FileMode.Open, FileAccess.Read);
		//已追加的方式 写入文件流
		FileStream toFile = new FileStream(toPath, FileMode.Append, FileAccess.Write);
		//实际读取的文件长度
		int toCopyLength = 0;
		//如果每次读取的长度小于 源文件的长度 分段读取
		if (eachReadLength < fromFile.Length)
		{
			byte[] buffer = new byte[eachReadLength];
			long copied = 0;
			while (copied <= fromFile.Length - eachReadLength)
			{
				toCopyLength = fromFile.Read(buffer, 0, eachReadLength);
				fromFile.Flush();
				toFile.Write(buffer, 0, eachReadLength);
				toFile.Flush();
				//流的当前位置
				toFile.Position = fromFile.Position;
				copied += toCopyLength;
				
			}
			int left = (int)(fromFile.Length - copied);
			toCopyLength = fromFile.Read(buffer, 0, left);
			fromFile.Flush();
			toFile.Write(buffer, 0, left);
			toFile.Flush();

		}
		else
		{
			//如果每次拷贝的文件长度大于源文件的长度 则将实际文件长度直接拷贝
			byte[] buffer = new byte[fromFile.Length];
			fromFile.Read(buffer, 0, buffer.Length);
			fromFile.Flush();
			toFile.Write(buffer, 0, buffer.Length);
			toFile.Flush();

		}
		fromFile.Close();
		toFile.Close();
		return true;
	}
 
	private static void CompressFileLZMA(string inFile, string outFile)
	{
		SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
		//下面的这个写法 适合小文件 如果文件太大会溢出
		FileStream input = new FileStream(inFile, FileMode.Open, FileAccess.Read);
		FileStream output = new FileStream(outFile, FileMode.Append, FileAccess.Write);

		//实际读取的文件长度
        int toCopyLength = 0;
		//每次读取的长度
		int eachReadLength = 8;
		//coder.WriteCoderProperties(output);
		if (eachReadLength < input.Length)
        {
			byte[] buffer = new byte[eachReadLength];
            long copied = 0;
			while (copied <= input.Length - eachReadLength)
            {
				toCopyLength = input.Read(buffer, 0, eachReadLength);
				input.Flush();
				output.Write(buffer, 0, eachReadLength);
                output.Flush();
				//流的当前位置
                output.Position = input.Position;
                copied += toCopyLength;
			}
			int left = (int)(input.Length - copied);
			toCopyLength = input.Read(buffer, 0, left);
			input.Flush();
			output.Write(buffer, 0, left);
			output.Flush();
		}
		else
		{
			//如果每次拷贝的文件长度大于源文件的长度 则将实际文件长度直接拷贝
            byte[] buffer = new byte[input.Length];
			input.Read(buffer, 0, buffer.Length);
			input.Flush();
			output.Write(buffer, 0, buffer.Length);
			output.Flush();
		}
		//coder.Code(input, output, input.Length, -1, null);//
		output.Close();//关闭当前流并释放与当前流关联的任何资源（如套接字和文件句柄）
		input.Close();
		input.Dispose();//释放流所有使用的资源
		output.Dispose();
	}
	
	private static void DecompressFileLZMA(string inFile, string outFile)
	{
		SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
		FileStream input = new FileStream(inFile, FileMode.Open);
		FileStream output = new FileStream(outFile, FileMode.Create);
		
		// Read the decoder properties
		byte[] properties = new byte[5];
		input.Read(properties, 0, 5);
		
		// Read in the decompress file size.
		byte [] fileLengthBytes = new byte[8];
		input.Read(fileLengthBytes, 0, 8);
		long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
 
		// Decompress the file.
		coder.SetDecoderProperties(properties);
		coder.Code(input, output, input.Length, fileLength, null);
		output.Flush();
		output.Close();
		input.Close();
		input.Dispose();
		output.Dispose();
	}
}
