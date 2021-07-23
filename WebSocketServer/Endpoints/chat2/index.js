let logger = require("logger");

function on_open(event) {
    console.log("on_open:");
    console.log(event);
    for (let item of Object.keys(event)) {
        logger.log(item);
    }
}

function on_close(event) {
    console.log("on_close:");
    console.log(event);
    for (let item of Object.keys(event)) {
        console.log(item);
    }
}

function on_message(event) {
    console.log("on_message:");

    if (event.istext) {
        console.log("Message: " + event.data);
        send("You wrote: " + event.data);
    }
}
