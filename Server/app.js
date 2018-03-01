var io = require('socket.io')(process.envPort||3000);
var shortid = require("shortid")//new

console.log("Server Started");
console.log(shortid.generate());//new

//var playerCount = 0;//removed
var players = [];//new

io.on('connection', function(socket){
	var thisPlayerID = shortid.generate();//new
	players.push(thisPlayerID);//new
	
	console.log('client connected spawning player with ID: ' + thisPlayerID);//new
	
	socket.broadcast.emit('spawn player', {id:thisPlayerID});//new
	//playerCount++;//remove
	
	// for(var i= 0; i<playerCount; i++){//remove
		// socket.emit('spawn player');
		// console.log("Adding a new player");
	// }
	
	players.forEach(function(playerID){//new
		if(playerID == thisPlayerID) return;
		
		socket.emit('spawn player', {id:playerID});
		console.log("Added player " + playerID + " already on server to instance for " + thisPlayerID);
	})
	
	socket.on('playerhere', function(data){
		console.log("Player is logged in!");
	});

	socket.on('move', function(data){//new
		data.id = thisPlayerID;
		console.log("Player position is: " + JSON.stringify(data))
		socket.broadcast.emit("move", data)
	});
	
	socket.on('disconnect', function(){
		console.log("Player " + thisPlayerID + " Disconnected");//new
		players.splice(players.indexOf(thisPlayerID), 1);//new
		socket.broadcast.emit("disconnected", {id:thisPlayerID});//new
		//playerCount--;//remove
	});
});