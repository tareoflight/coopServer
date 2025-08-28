# RequestBuilder.gd
class_name RequestBuilder

var stream
var data: PackedByteArray

func _init(_stream):
	stream = _stream

func with_data(_data: PackedByteArray) -> RequestBuilder:
	data = _data
	return self

func send():
	stream.send(data)
