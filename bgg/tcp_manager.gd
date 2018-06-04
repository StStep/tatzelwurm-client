extends Node

const PORT = 1337
const HOST = "127.0.0.1"
var client = StreamPeerTCP.new()

const TIMEOUT = 3

var timeout_counter = 0
var connect_attemps_counter = 0
var connected = false

func _ready():
	set_process(true)
	pass

func _process(delta):
	match client.get_status():
		StreamPeerTCP.STATUS_NONE, StreamPeerTCP.STATUS_ERROR:
			if connected:
				print('TCP: Disonnected')
				connected = false;
			if timeout_counter == 0:
				connect_attemps_counter += 1
				print("TCP: Trying to Connect to ",HOST,":",PORT," (",connect_attemps_counter,")")
				client.connect_to_host(HOST, PORT)
			else:
				timeout_counter += delta
				if timeout_counter >= TIMEOUT:
					timeout_counter = 0 # we would like to try to connect again
			pass
		StreamPeerTCP.STATUS_CONNECTING:
			pass
		StreamPeerTCP.STATUS_CONNECTED:
			if not connected:
				print('TCP: Connected')
				connected = true;
			var bytes = client.get_available_bytes()
			if bytes > 0:
				var data = client.get_data(bytes)
				print('TCP: RX, %d bytes - %s' % [bytes, data])
			pass
