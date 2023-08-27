using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zhibo.dpg
{
    public class WebSocketClient
    {
        private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();
        private readonly Uri _serverUri;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public event Action<string> MessageReceived;
        public event Action Connected;
        public event Action<string> ErrorOccurred;

        public WebSocketClient(string serverUrl)
        {
            _serverUri = new Uri(serverUrl);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _clientWebSocket.ConnectAsync(_serverUri, _cancellationTokenSource.Token);
                Connected?.Invoke();
                StartListening();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(ex.Message);
            }
        }

        private async void StartListening()
        {
            var buffer = new byte[1024];

            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    MessageReceived?.Invoke(message);
                }
            }
            catch (WebSocketException wsex) when (wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                // Handle connection closed error
                ErrorOccurred?.Invoke("Connection closed prematurely.");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(ex.Message);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_clientWebSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected.");
            }

            var buffer = Encoding.UTF8.GetBytes(message);
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }

        public async Task DisconnectAsync()
        {
            if (_clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client initiated close", _cancellationTokenSource.Token);
            }
        }
    }
}