using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.IO;

namespace DashboardDesempenho {
    internal class Program {
        static void Main(string[] args) {
            CreateTitle();
            ShowPerformanceOfProcesses();

            FinalizeProgram();
        }

        private static void ShowPerformanceOfProcesses() {
            //CPU
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //RAM
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            //Disco
            DriveInfo[] drivers = DriveInfo.GetDrives();
            PerformanceCounter diskReadBytesPSec = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            PerformanceCounter diskWriteBytesPSec = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
            PerformanceCounter diskAvgRead = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Read", "_Total");
            PerformanceCounter diskAvgWrite = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Write", "_Total");
            //Rede
            var interfaces = new PerformanceCounterCategory("Network Interface").GetInstanceNames();
            PerformanceCounter bytesReceivedCounter = new PerformanceCounter();
            PerformanceCounter bytesSentCounter = new PerformanceCounter();

            //Pegar valores e usar sleep para evitar valores nulos
            cpuCounter.NextValue();
            ramCounter.NextValue();
            Thread.Sleep(1000);



            while (!Console.KeyAvailable) {
                /*Valores CPU*/
                float cpuUsage = cpuCounter.NextValue();

                /*Valores Memória*/
                float totalRAM = GetTotalPhysicalMemory();//const
                float availableRAM = ramCounter.NextValue();
                float availableRAMpcent = (availableRAM / totalRAM) * 100; // %
                float usedRAM = totalRAM - availableRAM;
                float usedRAMpcent = (usedRAM / totalRAM) * 100; // %

                /*Valores Disco*/
                float ReadBytesPSec = diskReadBytesPSec.NextValue();
                float WriteBytesPSec = diskWriteBytesPSec.NextValue();
                float avgReadSec = diskAvgRead.NextValue();
                float avgWriteSec = diskAvgWrite.NextValue();
                //Transformação dos valores
                float ReadMBytesPSec = ReadBytesPSec / (1024 * 1024);
                float WriteMBytesPSec = WriteBytesPSec / (1024 * 1024);
                float avgReadMs = avgReadSec * 1000;
                float avgWriteMs = avgWriteSec * 1000;


                // Display 
                Console.SetCursorPosition(0, 18); //Atualizar o console a partir da linha 19

                //CPU
                Console.WriteLine($"CPU Usage: {cpuUsage:F2}%\n");

                //RAM
                Console.WriteLine($"Total RAM: {totalRAM:F2} MB");
                Console.WriteLine($"Available RAM: {availableRAM:F2} MB  -  ({availableRAMpcent:F2}%)");
                Console.WriteLine($"Used RAM: {usedRAM:F2} MB  -  ({usedRAMpcent:F2}%)\n");

                //DISCO
                foreach(DriveInfo drive in drivers) {
                    //const
                    Console.WriteLine($"Space Available {drive.Name.Substring(0,1)}: {drive.TotalFreeSpace / (1024 * 1024 * 1024):F2}/{drive.TotalSize / (1024 * 1024 * 1024):F2} GB");
                }
                Console.WriteLine($"Read (MB p/ sec): {ReadMBytesPSec:F2}");
                Console.WriteLine($"Avg. Disk Read Time: {avgReadSec:F6} sec ({avgReadMs:F3} ms)");
                Console.WriteLine($"Write (MB p/ sec): {WriteMBytesPSec:F2}");
                Console.WriteLine($"Avg. Disk Write Time: {avgWriteSec:F6} sec ({avgWriteMs:F3} ms)");


                // sleep de 1 seg p atualizar valores
                Thread.Sleep(1000);
            }
        }


        //Retorna memória total da maquina
        static float GetTotalPhysicalMemory() {
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            return (float)(computerInfo.TotalPhysicalMemory / (1024 * 1024)); // Convert bytes to MB
        }

        private static void CreateTitle() {
            Console.Clear();
            Console.WriteLine("\r\n██████╗░███████╗██████╗░███████╗░█████╗░██████╗░███╗░░░███╗░█████╗░███╗░░██╗░█████╗░███████╗\r\n██╔══██╗██╔════╝██╔══██╗██╔════╝██╔══██╗██╔══██╗████╗░████║██╔══██╗████╗░██║██╔══██╗██╔════╝\r\n██████╔╝█████╗░░██████╔╝█████╗░░██║░░██║██████╔╝██╔████╔██║███████║██╔██╗██║██║░░╚═╝█████╗░░\r\n██╔═══╝░██╔══╝░░██╔══██╗██╔══╝░░██║░░██║██╔══██╗██║╚██╔╝██║██╔══██║██║╚████║██║░░██╗██╔══╝░░\r\n██║░░░░░███████╗██║░░██║██║░░░░░╚█████╔╝██║░░██║██║░╚═╝░██║██║░░██║██║░╚███║╚█████╔╝███████╗\r\n╚═╝░░░░░╚══════╝╚═╝░░╚═╝╚═╝░░░░░░╚════╝░╚═╝░░╚═╝╚═╝░░░░░╚═╝╚═╝░░╚═╝╚═╝░░╚══╝░╚════╝░╚══════╝\r\n\r\n██████╗░░█████╗░░██████╗██╗░░██╗██████╗░░█████╗░░█████╗░██████╗░██████╗░\r\n██╔══██╗██╔══██╗██╔════╝██║░░██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗\r\n██║░░██║███████║╚█████╗░███████║██████╦╝██║░░██║███████║██████╔╝██║░░██║\r\n██║░░██║██╔══██║░╚═══██╗██╔══██║██╔══██╗██║░░██║██╔══██║██╔══██╗██║░░██║\r\n██████╔╝██║░░██║██████╔╝██║░░██║██████╦╝╚█████╔╝██║░░██║██║░░██║██████╔╝\r\n╚═════╝░╚═╝░░╚═╝╚═════╝░╚═╝░░╚═╝╚═════╝░░╚════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░");
            Console.WriteLine("\r\n█▄▄ █▄█ ▀   █▀▀ █▀█ █▀█ █▀▀ █░█░█ █░█░█ █░█░█\r\n█▄█ ░█░ ▄   █▄█ █▀▄ █▄█ █▄█ ▀▄▀▄▀ ▀▄▀▄▀ ▀▄▀▄▀\n\n");
        }
        private static void FinalizeProgram() {
            Console.WriteLine("\nPressione qualquer tecla para finalizar o programa...\n");
            Console.ReadKey();
        }
    }
}
