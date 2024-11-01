using System;
using System.Net.Sockets;
using System.Threading.Tasks;

class ModbusRtuOverTcpClient
{
    private TcpClient _tcpClient;
    private NetworkStream _stream;

    public async Task ConnectAsync(string ipAddress, int port)
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(ipAddress, port);
        _stream = _tcpClient.GetStream();
    }

    public async Task<byte[]> ReadRegistersAsync(byte deviceAddress, ushort startAddress, ushort numRegisters)
    {
        // Формируем RTU-запрос без CRC
        byte[] request = new byte[6];
        request[0] = deviceAddress; // Адрес устройства
        request[1] = 3; // Код функции (Read Holding Registers)
        request[2] = (byte)(startAddress >> 8); // Начальный адрес (старший байт)
        request[3] = (byte)(startAddress & 0xFF); // Начальный адрес (младший байт)
        request[4] = (byte)(numRegisters >> 8); // Количество регистров (старший байт)
        request[5] = (byte)(numRegisters & 0xFF); // Количество регистров (младший байт)

        // Отправляем запрос
        await _stream.WriteAsync(request, 0, request.Length);

        // Читаем ответ
        byte[] response = new byte[5 + 2 * numRegisters];
        int bytesRead = await _stream.ReadAsync(response, 0, response.Length);

        if (bytesRead != response.Length)
        {
            throw new Exception("Неполный ответ от устройства.");
        }

        return response;
    }

    public void Close()
    {
        _stream?.Close();
        _tcpClient?.Close();
    }
}
