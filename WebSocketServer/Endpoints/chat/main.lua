-- main.lua


function on_open()
	print("New session: " .. session.id)
	websocket.send(string.format("Hello user (%s).", session.id))
end


function on_close(event)
	print("Session " .. session.id .. " was closed:")
	print(string.format("Code: %d, Reason: %s", event.code, event.reason))

	websocket.send(string.format("Good bye, %s.", session.id))
end

function on_message(msg)

	print(string.format("Message from %s:", session.id))
	print(string.format("Message Content: %s", msg.data))
	local result = json.parse(msg.data)

	websocket.send("You wrote: " .. result.content)
end
