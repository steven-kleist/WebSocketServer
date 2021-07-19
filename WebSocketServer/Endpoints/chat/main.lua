-- main.lua



function on_open(event)
	print("New session: " .. event.session_id)
end


function on_close(event)
	print("Session closed: " .. event.session_id)
end

function on_message(event)

	print("Session: " .. event.session_id)
	print("Data: " .. event.data)
	local result = json.parse(event.data)

	send("You wrote: " .. result.content)
end
