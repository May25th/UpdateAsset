using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SevenZip.Compression.LZMA;
using System.IO;
using System;

/// <summary>
/// 雨松博客的实现
/// http://www.xuanyusong.com/archives/3095
/// </summary>
public class TestLZMA:Editor 
{
    [MenuItem ("MyMenu/CompressFile")]
	static void CompressFile () 
	{
		//压缩文件
		//Application.dataPath 编辑器环境下是 Asset文件夹目录
		CompressFileLZMA(Application.dataPath+"/1.jpg",Application.dataPath+"/2.zip");
		AssetDatabase.Refresh();
 
	}
	[MenuItem ("MyMenu/DecompressFile")]
	static void DecompressFile () 
	{
		//解压文件
		DecompressFileLZMA(Application.dataPath+"/2.zip",Application.dataPath+"/3.jpg");
		AssetDatabase.Refresh();
	}
 
 
	private static void CompressFileLZMA(string inFile, string outFile)
	{
		SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
		/* 
			FileMode 以何种方式打开或者创建文件
			FileMode.Open 打开
			FileMode.Create 创建并覆盖
			FileMode.CreateNew 创建新文件
			FileMode.OpenOrCreate 打开并创建
			FileMode.Truncate 覆盖文件
			FileMode.Append 追加

			FileAcess 文件流对象如何访问该文件
			FileAcess.Read 只读
			FileAcess.Write 写
			FileAcess.ReadWirte 读写

			FileShare 进程如何共享文件
			FileShare.None 拒绝共享
			Read 、Write、ReadWrite（同时读写）、Delete
			//用法
			FileStream logFile = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)
		 */

		//下面的这个写法 适合小文件 如果文件太大会溢出
		FileStream input = new FileStream(inFile, FileMode.Open);
		FileStream output = new FileStream(outFile, FileMode.Create);
		coder.WriteCoderProperties(output);
		//写入内存
		output.Write(BitConverter.GetBytes(input.Length), 0, 8);//使用缓冲区中的数据将字节块写入此流
		coder.Code(input, output, input.Length, -1, null);//
		//写入硬盘
		output.Flush();//清除缓冲区，把所有数据写入文件中
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
