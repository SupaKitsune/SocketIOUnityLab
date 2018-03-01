var io = require('socket.io')(process.envPort||3000)
var shortid = require("shortid")
var MongoClient = require("mongodb").MongoClient//new
var url = "mongodb://localhost:27017/"//new

var players = [];

var dbObj;

console.log("Server Started");

MongoClient.connect(url, function(error, client){
	if(error)throw error;

	dbObj = client.db("SocketLabData");

	console.log("Database connected");
})

//console.log(shortid.generate());

io.on('connection', function(socket){
	var thisPlayerID = shortid.generate();
	players.push(thisPlayerID);
	
	console.log('client connected spawning player with ID: ' + thisPlayerID);
	
	socket.broadcast.emit('spawn player', {id:thisPlayerID});
	//playerCount++;//remove
	
	// for(var i= 0; i<playerCount; i++){//remove
		// socket.emit('spawn player');
		// console.log("Adding a new player");
	// }
	
	players.forEach(function(playerID){
		if(playerID == thisPlayerID) return;
		
		socket.emit('spawn player', {id:playerID});
		console.log("Added player " + playerID + " already on server to instance for " + thisPlayerID);
	})
	
	socket.on('playerhere', function(data){
		console.log("Player is logged in!");
	});

	socket.on('move', function(data){
		data.id = thisPlayerID;
		console.log("Player position is: " + JSON.stringify(data))
		socket.broadcast.emit("move", data)
	});
	
	socket.on('disconnect', function(){
		console.log("Player " + thisPlayerID + " Disconnected");
		players.splice(players.indexOf(thisPlayerID), 1);
		socket.broadcast.emit("disconnected", {id:thisPlayerID});
		//playerCount--;//remove
	});

	socket.on("data pass", function(data){
		console.log(JSON.stringify(data))

		dbObj.collection("playerData").save(data, function(error, response){
			if(error)throw error;
			console.log("saved data to server")
		})
	})
});