-- main.lua

for k,v in pairs(package) do
	print(k,v)
end

local logger = require "logger"
--print(package.path)


function on_open(event)
	logger.log("New session: " .. event.session_id)
end


function on_close(event)
	print("Session closed: " .. event.session_id)
end

function on_message(event)

	print("type(event): " .. type(event))
	print("Session: " .. event.session_id)
	print("Data: " .. event.data)
	local result = json.parse(event.data)

	send("You wrote: " .. result.content)
end
