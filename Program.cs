using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var modbusClient = new ModbusRtuOverTcpClient();
        await modbusClient.ConnectAsync("127.0.0.1", 502); // IP-адрес и порт

        byte deviceAddress = 1;
        ushort startAddress = 0;
        ushort numRegisters = 10;

        try
        {
            byte[] response = await modbusClient.ReadRegistersAsync(deviceAddress, startAddress, numRegisters);
            Console.WriteLine("Received data: " + BitConverter.ToString(response));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
        finally
        {
            modbusClient.Close();
        }
    }
}
