using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPFrameMonitorWithNetwork
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine("start monitoring ...");

            int minuts = 0;


            while (true)
            {
                // (1) IP 주소와 포트를 지정하고 TCP 연결 
                TcpClient tc = new TcpClient("52.141.61.156", 8000);
                //TcpClient tc = new TcpClient("127.0.0.1", 8000);
                //TcpClient tc = new TcpClient("localhost", 7000);


                string msg = "select sum(i), sum(p) from Frames WHERE TimeStamp > datetime('now', 'localtime', '-1 minutes');";
                byte[] buff = Encoding.ASCII.GetBytes(msg);

                // (2) NetworkStream을 얻어옴 
                NetworkStream stream = tc.GetStream();

                // (3) 스트림에 바이트 데이타 전송
                stream.Write(buff, 0, buff.Length);

                // (4) 스트림으로부터 바이트 데이타 읽기
                byte[] outbuf = new byte[1024];
                int nbytes = stream.Read(outbuf, 0, outbuf.Length);
                string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);

                // (5) 스트림과 TcpClient 객체 닫기
                stream.Close();
                tc.Close();

                Console.WriteLine($"{nbytes} bytes: {output}");


                Thread.Sleep(1000 * 60);    // 1 minute
                minuts++;
                
                
                //60분마다 2시간 이전의 data를 삭제한다.
                //todo 60분마다 messagebox로 통계값을 보여준다.
                if ( minuts == 60 )
                {
                    // (2) NetworkStream을 얻어옴 
                    stream = tc.GetStream();
                    //delete
                    msg = "delete from Frames where TimeStamp < datetime('now', 'localtime', '-120 minutes');";
                    buff = Encoding.ASCII.GetBytes(msg);
                    // (3) 스트림에 바이트 데이타 전송
                    stream.Write(buff, 0, buff.Length);

                    // (4) 스트림으로부터 바이트 데이타 읽기
                    nbytes = stream.Read(outbuf, 0, outbuf.Length);
                    output = Encoding.ASCII.GetString(outbuf, 0, nbytes);

                    stream.Close();
                    tc.Close();

                    Console.WriteLine($"{nbytes} bytes: {output}");

                    minuts = 0;

                }
                

            }
        }

    }
}

