import { createRequire } from "module";
const require = createRequire(import.meta.url);

const THREE = require('three')
import { PhysicsEngine } from './PhysicEngine.js'

const engine = await PhysicsEngine.initialize();
engine.createScene();

let rope_number;

let is_grabbed;

let has_the_simulation_started = false;

let client_number = 0;
let client_message_id = new Map();

let server_message_id = 0;

let rope_sphere_positions = [];
let rope_minisphere1_positions = [];
let rope_minisphere2_positions = [];
let rope_minisphere3_positions = [];
let rope_minisphere4_positions = [];
let rope_minisphere5_positions = [];
let rope_minisphere6_positions = [];
let rope_minisphere7_positions = [];
let initial_positions = [];

let board_position;
let board_rotation;

let cubes_positions = [];
let cubes_rotations = [];
let cubes_initial_positions = [];
let cubes_initial_rotations = [];


const WebSocket = require('ws')
const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('server started')
})
wss.on('connection', function connection(ws) {
   console.log("client connected");
   
   // else 
   client_number++;
   ws.on('message', (data) => {
      // console.log(data.toString())
      let parsed_data = data.toString().replaceAll(",", ".").split(";");
      let vec;
      let id = parseFloat(parsed_data[1]);
      switch(parsed_data[0])
      {
         case "rope_number":
            client_message_id.set(ws, id);
            rope_number = parseInt(parsed_data[2]);
            is_grabbed = Array(rope_number).fill(false);
            engine.createRopes(rope_number);
            break;
         case "grabbed":
            client_message_id.set(ws, id);
            is_grabbed[parseInt(parsed_data[2])] = true;
            engine.setRopeGravity(0, parseInt(parsed_data[2]));
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("grabbed" + ";" + parsed_data[2]);
               }
            });
            break;
         case "released":
            client_message_id.set(ws, id);
            is_grabbed[parseInt(parsed_data[5])] = false;
            //engine.setRopeVelocity(new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4])), parseInt(parsed_data[5]));
            engine.setRopeGravity(1, parseInt(parsed_data[5]));
            wss.clients.forEach(function each(client) {
               if(client.readyState === WebSocket.OPEN)
               {
                  client.send("released" + ";" + parsed_data[5]);
               }
            });
            break;
         case "positions_grabbed":
            if( id > client_message_id.get(ws)) {
               client_message_id.set(ws, id);
               vec = new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4]));
               rope_sphere_positions[parseInt(parsed_data[5])] = vec.clone();
               engine.setRope(vec, parseInt(parsed_data[5]));
               // engine.setRopeVelocity(new THREE.Vector3(parseFloat(parsed_data[2]), parseFloat(parsed_data[3]), parseFloat(parsed_data[4])), parseInt(parsed_data[5]));
            }
            break;
         case "reset":
            console.log("reset");
            client_message_id.set(ws, id);
            engine.reset(rope_number);
            for (let i = 0; i < 14; i++)
            {
               engine.setCube(cubes_initial_positions[i], new THREE.Quaternion(0,0,0,1), i);
            }
            console.log("fin du reset");
            break;
         case "add_cube":
            engine.addCube(new THREE.Vector3(parseFloat(parsed_data[3]), parseFloat(parsed_data[4]), parseFloat(parsed_data[5])), parseInt(parsed_data[2]));
            cubes_initial_positions[parseInt(parsed_data[2])] = new THREE.Vector3(parseFloat(parsed_data[3]), parseFloat(parsed_data[4]), parseFloat(parsed_data[5]));
            break;
         case "start":
            engine.manageLevel(parseInt(parsed_data[2]));
            if (!has_the_simulation_started)
            {
               has_the_simulation_started = true;
               startLoop();
            }
            break;
         case "leaving":
            console.log("leaving")
            client_number--;
            client_message_id.set(ws, 0);
            if (client_number == 0)
            {
               stopLoop();
               has_the_simulation_started = false;
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


function loop( ) {
   engine.step();
   server_message_id++;
   let message = server_message_id + ";";
   // for (let i = 0; i < 4; i++)
   // {
   //    if (!is_grabbed[i])
   //    {
   //       rope_sphere_positions[i] = engine.getBallPosition(i);
   //    }
   //    message += rope_sphere_positions[i].x + ";" + rope_sphere_positions[i].y + ";" + rope_sphere_positions[i].z + ";";
   // }
   for (let i = 0; i < rope_number; i++)
   {
      if (!is_grabbed[i])
      {
         rope_sphere_positions[i] = engine.getRopeSpherePosition(i);
      }
      message += rope_sphere_positions[i].x + ";" + rope_sphere_positions[i].y + ";" + rope_sphere_positions[i].z + ";";
      rope_minisphere1_positions[i] = engine.getRopeMinispherePosition(i, 0);
      message += rope_minisphere1_positions[i].x + ";" + rope_minisphere1_positions[i].y + ";" + rope_minisphere1_positions[i].z + ";";
      rope_minisphere2_positions[i] = engine.getRopeMinispherePosition(i, 1);
      message += rope_minisphere2_positions[i].x + ";" + rope_minisphere2_positions[i].y + ";" + rope_minisphere2_positions[i].z + ";";
      rope_minisphere3_positions[i] = engine.getRopeMinispherePosition(i, 2);
      message += rope_minisphere3_positions[i].x + ";" + rope_minisphere3_positions[i].y + ";" + rope_minisphere3_positions[i].z + ";";
      rope_minisphere4_positions[i] = engine.getRopeMinispherePosition(i, 3);
      message += rope_minisphere4_positions[i].x + ";" + rope_minisphere4_positions[i].y + ";" + rope_minisphere4_positions[i].z + ";";
      rope_minisphere5_positions[i] = engine.getRopeMinispherePosition(i, 4);
      message += rope_minisphere5_positions[i].x + ";" + rope_minisphere5_positions[i].y + ";" + rope_minisphere5_positions[i].z + ";";
      rope_minisphere6_positions[i] = engine.getRopeMinispherePosition(i, 5);
      message += rope_minisphere6_positions[i].x + ";" + rope_minisphere6_positions[i].y + ";" + rope_minisphere6_positions[i].z + ";";
      rope_minisphere7_positions[i] = engine.getRopeMinispherePosition(i, 6);
      message += rope_minisphere7_positions[i].x + ";" + rope_minisphere7_positions[i].y + ";" + rope_minisphere7_positions[i].z + ";";
   }

   board_position = engine.getBoardPosition();
   message += board_position.x + ";" + board_position.y + ";" + board_position.z + ";";
   board_rotation = engine.getBoardRotation();
   message += board_rotation.x + ";" + board_rotation.y + ";" + board_rotation.z + ";" + board_rotation.w + ";";

   for (let i = 0; i < 14; i++)
   {
      cubes_positions[i] = engine.getCubePosition(i);
      cubes_rotations[i] = engine.getCubeRotation(i);
      message += cubes_positions[i].x + ";" + cubes_positions[i].y + ";" + cubes_positions[i].z + ";";
      message += cubes_rotations[i].x + ";" + cubes_rotations[i].y + ";" + cubes_rotations[i].z + ";" + cubes_rotations[i].w + ";";
   }
   
   wss.clients.forEach(function each(client) {
      if(client.readyState === WebSocket.OPEN)
      {
         client.send(message);
      }
   });
}


let interval;
function startLoop ( ) {
   engine.start( )
   interval = setInterval(loop, 1000/120)
}

function stopLoop ( ) {
   clearInterval(interval);
}