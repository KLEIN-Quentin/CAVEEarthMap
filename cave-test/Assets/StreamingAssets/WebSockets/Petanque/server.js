import { createRequire } from "module";
const require = createRequire(import.meta.url);

const THREE = require('three')
import { PhysicsEngine } from './PhysicEngine.js'

const engine = await PhysicsEngine.initialize();
engine.createScene();

class Ball 
{
   constructor(pos, vel)
   {
      this.radius = 0.0175;
      this.mass = 0.0027;
      this.pos = pos.clone();
      this.pos_1 = pos.clone();
      this.vel = vel.clone();
   }
}

let ball = new Ball(new THREE.Vector3(-2.5, 1.45, -6), new THREE.Vector3())

let is_grabbed = Array(12).fill(false);

let has_the_simulation_started = false;

let client_number = 0;
let client_message_id = new Map();

let server_message_id = 0;

const clock = new THREE.Clock(false);

let ball_number = 0;

let initialzed_ball = 0;

let ball_positions = [];
let initial_positions = [];

// Start the connection
const WebSocket = require('ws')
const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('server started')
})
wss.on('connection', function connection(ws) {
   console.log("client connected");

   client_number++;
   // starts the simulation loop
   if (!has_the_simulation_started)
   {
      has_the_simulation_started = true;
      clock.start();
      startLoop();
   }
   ws.on('message', (data) => {
      let parsed_data = data.toString().replaceAll(",", ".").split(";");
      let vec;
      let id = parseFloat(parsed_data[1]);
      switch(parsed_data[0])
      {
         // when a ball has been grabbed, sends the information to every clients
         case "grabbed":
            client_message_id.set(ws, id);
            is_grabbed[parseInt(parsed_data[2])] = true;
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("grabbed" + ";" + parsed_data[2]);
               }
            });
            break;
         // when a ball has been released, updates the velocity and sends the informations to every clients
         case "released":
            client_message_id.set(ws, id);
            is_grabbed[parseInt(parsed_data[5])] = false;
            engine.setBallVelocity(new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4])), parseInt(parsed_data[5]));
            
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("released" + ";" + parsed_data[5]);
               }
            });
            break;
         // when a ball is being grabbed, receives the new position of the ball
         case "positions_grabbed":
            if( id > client_message_id.get(ws)) {
               client_message_id.set(ws, id);
               clock.getDelta();
               vec = new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4]));
               ball_positions[parseInt(parsed_data[5])] = vec.clone();
               engine.setBall(vec, parseInt(parsed_data[5]));
            }
            break;
         // when a new ball is added, creates the ball with its initial position
         case "add_ball":
            console.log("new_ball");
            vec = new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4]));
            initial_positions[parseInt(parsed_data[1])] = vec.clone();
            engine.addBall(vec, parseInt(parsed_data[1]))
            initialzed_ball++;
            if (initialzed_ball == 7) {
               ball_number = 7;
            }
            else if (initialzed_ball == 13)
            {
               ball_number = 13
            }
            break;
         // when more balls are needed (with the addition of a 4th player), sends the information to every clients
         case "more_balls":
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("more_balls");
               }
            });
            break;
         // resets the balls to their initial positions
         case "reset_ball":
            console.log("reset");
            client_message_id.set(ws, id);
            ball_positions[parseInt(parsed_data[2])] = initial_positions[parseInt(parsed_data[2])].clone();
            engine.setBall(initial_positions[parseInt(parsed_data[2])], parseInt(parsed_data[2]));
            engine.setBallVelocity(new THREE.Vector3(0,0,0), parseInt(parsed_data[2]));
            break;
         // sends the new score to every clients
         case "update_score":
            console.log("updating scores");
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("update_score;" + parsed_data[1] + ";" + parsed_data[2]);
               }
            });
            break;
         case "leaving":
            console.log("leaving")
            client_number--;
            client_message_id.set(ws, 0);
            // if there is only one client left, that means its the Unity server, so the simulation is stopped and reinitialized
            if (client_number == 1)
            {
               stopLoop();
               has_the_simulation_started = false;
               engine.removeBalls();
               ball_number = 0;
               initialzed_ball = 0;
            }
            break;
         default:
            break;
      }
   })
})
wss.on('listening',()=>{
   console.log('listening on 8080')
})

// called each frame
function loop( ) {
   engine.step();
   server_message_id++;
   let message = server_message_id + ";";
   // adds the positions of each ball in the message
   for (let i = 0; i < ball_number; i++)
   {
      if (!is_grabbed[i])
      {
         ball_positions[i] = engine.getBallPosition(i);
      }
      message += ball_positions[i].x + ";" + ball_positions[i].y + ";" + ball_positions[i].z + ";";
   }
   // sends the message to every clients
   wss.clients.forEach(function each(client) {
      if(client.readyState === WebSocket.OPEN)
      {
         client.send(message);
      }
   });
}


let interval;
function startLoop ( ) {
   engine.start()
   interval = setInterval(loop, 1000/120) // 120 fps
}

function stopLoop ( ) {
   clearInterval(interval);
}