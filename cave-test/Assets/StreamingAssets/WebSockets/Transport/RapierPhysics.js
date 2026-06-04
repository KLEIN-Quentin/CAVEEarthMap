import { Clock, Vector3, Quaternion, Matrix4 } from 'three';

import RAPIER from '@dimforge/rapier3d-compat';

const frameRate = 120;

const _scale = new Vector3( 1, 1, 1 );
const ZERO = new Vector3();

//let RAPIER = null;

function getShape( geometry ) {

	const parameters = geometry.parameters;

	// TODO change type to is*

	if ( geometry.type === 'BoxGeometry' ) {

		const sx = parameters.width !== undefined ? parameters.width / 2 : 0.5;
		const sy = parameters.height !== undefined ? parameters.height / 2 : 0.5;
		const sz = parameters.depth !== undefined ? parameters.depth / 2 : 0.5;

		return RAPIER.ColliderDesc.cuboid( sx, sy, sz );

	} else if ( geometry.type === 'SphereGeometry' || geometry.type === 'IcosahedronGeometry' ) {

		const radius = parameters.radius !== undefined ? parameters.radius : 1;
		return RAPIER.ColliderDesc.ball( radius );

	} else if ( geometry.type === 'CylinderGeometry' ) {

		const radius = parameters.radiusTop !== undefined ? parameters.radiusTop : 1;
		return RAPIER.ColliderDesc.cylinder( parameters.height / 2, radius );

	} else if ( geometry.type === 'CapsuleGeometry' ) {

		const radius = parameters.radius !== undefined ? parameters.radius : 1;
		return RAPIER.ColliderDesc.capsule( parameters.length / 2, radius )

	} else if ( geometry.type === 'BufferGeometry' ) {

		const vertices = [];
		const vertex = new Vector3();
		const position = geometry.getAttribute( 'position' );

		for ( let i = 0; i < position.count; i ++ ) {

			vertex.fromBufferAttribute( position, i );
			vertices.push( vertex.x, vertex.y, vertex.z );

		}

		// if the buffer is non-indexed, generate an index buffer
		const indices = geometry.getIndex() === null
			? Uint32Array.from( Array( parseInt( vertices.length / 3 ) ).keys() )
			: geometry.getIndex().array;

		return RAPIER.ColliderDesc.trimesh( vertices, indices );

	}

	return null;

}

async function RapierPhysics() {

	if (!RAPIER) {
		console.log("Rapier n'a pas été chargé correctement.");
	} else {
		await RAPIER.init();
		console.log("Rapier initialisé !");
	}

	// Docs: https://rapier.rs/docs/api/javascript/JavaScript3D/

	const gravity = new Vector3( 0.0, - 9.81, 0.0 );
	const world = new RAPIER.World( gravity );

	const meshes = [];
	const kinematicMeshes = [];
	const meshMap = new WeakMap();

	const _vector = new Vector3();
	const _quaternion = new Quaternion();
	const _matrix = new Matrix4();

	function addScene( scene ) {

		scene.traverse( function ( child ) {

			if ( child.isMesh ) {
				const physics = child.userData.physics;

				if ( physics ) {
					addMesh( child, physics.mass, physics.restitution, physics.kinematic, physics.friction, physics.linearDampling ,physics.angularDampling );

				}

			}

		} );

	}

	function addMesh( mesh, mass = 0, restitution = 0, kinematic = false, friction = 0, linearDamping = 0 , angularDamping = 0) {

		const shape = getShape( mesh.geometry );

		if ( shape === null ) return;

		shape.setMass( mass );
		shape.setRestitution( restitution );
		shape.setFriction( friction );

		let body;
		if( kinematic ) {
			body = createKinematicBody( mesh, shape );
			kinematicMeshes.push(mesh);
		} else {
			body = createBody( mesh.position, mesh.quaternion, mass, shape );
			body.setLinearDamping(linearDamping);
			body.setAngularDamping(angularDamping);
			meshes.push( mesh );
		}
		meshMap.set( mesh, body );

		// if ( mass > 0 || kinematic ) {

		// 	meshes.push( mesh );
		// 	meshMap.set( mesh, body );

		// }
	}

	function removeMesh(mesh, kinematic) {
	
		let array = kinematic ? kinematicMeshes : meshes;
		const index = array.indexOf(mesh);
		
		if (index !== -1) {
			array.splice(index, 1);
			console.log("Mesh supprimé du tableau !");
		} else {
			console.log("Mesh introuvable dans le tableau !");
		}
	
		if (meshMap.has(mesh)) {
			const body = meshMap.get(mesh);
			world.removeRigidBody(body);  // Supprimer le corps physique
			meshMap.delete(mesh);
			console.log("Mesh supprimé du monde physique !");
		}
	
		if (mesh.parent) {
			mesh.parent.remove(mesh);
			console.log("Mesh retiré de la scène !");
		} else {
			console.log("Le mesh n'a pas de parent !");
		}
	}
	

	function createKinematicBody ( mesh, shape ) {
		const desc = RAPIER.RigidBodyDesc.kinematicPositionBased();
		desc.setTranslation( ...mesh.position );
		desc.setRotation( mesh.quaternion );
		const body = world.createRigidBody( desc );
		world.createCollider( shape, body );
		return body;
	}

	function createBody( position, quaternion, mass, shape ) {

		const desc = mass > 0 ? RAPIER.RigidBodyDesc.dynamic() : RAPIER.RigidBodyDesc.fixed();
		desc.setTranslation( ...position );
		if ( quaternion !== null ) desc.setRotation( quaternion );

		const body = world.createRigidBody( desc );
		world.createCollider( shape, body );

		return body;

	}

	function setMeshPosition( mesh, position ) {

		let body = meshMap.get( mesh );

		body.setAngvel( ZERO );
		body.setLinvel( ZERO );
		body.setTranslation( position );

	}

	function setMeshRotation( mesh, rotation) {

		let body = meshMap.get( mesh );
		body.setRotation(rotation);
	}

	function getMeshRotation (mesh) {
		return mesh.quaternion;
	}

	function setMeshVelocity( mesh, velocity ) {

		let body = meshMap.get( mesh );
		body.setLinvel( velocity );

	}

	function setMeshVelocityFromPos( mesh, pos, pos_1 ) {
		console.log('setMeshVelocityFromPos')
		const velocity = pos.clone().sub(pos_1).multiplyScalar(1/world.timestep);
		body.setTranslation( Vector3(-6, 2.23, -5.627565) );

		let body = meshMap.get( mesh );
		// body.setLinvel( velocity );
		body.setLinvel( new Vector3(0, 20, 0) );
		console.log('setMeshVelocityFromPos')
	}

	function setMeshGravity ( mesh, gravity ) {
		let body = meshMap.get( mesh );
		body.setGravityScale(gravity, true);
		console.log("gravité à " + gravity);
	}

	function sphericalJoinLink(meshA, meshB, anchorA, anchorB) {

		const bodyA = meshMap.get(meshA);
        const bodyB = meshMap.get(meshB);
        //bodyB.setAngularDamping( 1.0 );

		// const jointParams = RAPIER.JointData.rope(
		// 	0.01,
		// 	new RAPIER.Vector3(anchorA.x, anchorA.y, anchorA.z),
		// 	new RAPIER.Vector3(anchorB.x, anchorB.y, anchorB.z)
		// );

		const jointParams = RAPIER.JointData.spherical(
			new RAPIER.Vector3(anchorA.x, anchorA.y, anchorA.z),
			new RAPIER.Vector3(anchorB.x, anchorB.y, anchorB.z)
		);
		jointParams.damping = 40;
		//jointParams.stiffness = 30;

		// const jointParams = RAPIER.JointData.spring(
		// 	0.01, 3000, 300,
		// 	new RAPIER.Vector3(anchorA.x, anchorA.y, anchorA.z),
		// 	new RAPIER.Vector3(anchorB.x, anchorB.y, anchorB.z)
		// );

		const joint = world.createImpulseJoint(jointParams, bodyA, bodyB, true);
		//joint.setContactsEnabled(false);
		return joint;
	}

	function fixedJoinLink(meshA, meshB, anchorA, anchorB) {
		const bodyA = meshMap.get(meshA);
        const bodyB = meshMap.get(meshB);

		const jointParams = RAPIER.JointData.fixed(
			new RAPIER.Vector3(anchorA.x, anchorA.y, anchorA.z), { w: 1.0, x: 0.0, y: 0.0, z: 0.0 },
			new RAPIER.Vector3(anchorB.x, anchorB.y, anchorB.z), { w: 1.0, x: 0.0, y: 0.0, z: 0.0 }
		);

		const joint = world.createImpulseJoint(jointParams, bodyA, bodyB, true);
		return joint;
	}

	//

	const clock = new Clock(false);

	function step() {
		//
		for ( let i = 0, l = kinematicMeshes.length; i < l; i ++ ) {

			const mesh = kinematicMeshes[ i ];

			const body = meshMap.get( mesh );
			body.setNextKinematicTranslation(mesh.position);
			body.setNextKinematicRotation(mesh.quaternion);

		}

		world.timestep = clock.getDelta();
		world.step();


		for ( let i = 0, l = meshes.length; i < l; i ++ ) {
			const mesh = meshes[ i ];
			const body = meshMap.get( mesh );
			mesh.position.copy( body.translation() );
			mesh.quaternion.copy( body.rotation() );
		}
	}

	function start () {
		clock.start();
		// setInterval( step, 1000 / frameRate );
	}

	return {
		start: start,
		step: step,
		addScene: addScene,
		addMesh: addMesh,
		removeMesh: removeMesh,
		setMeshPosition: setMeshPosition,
		setMeshRotation: setMeshRotation,
		setMeshVelocity: setMeshVelocity,
		setMeshVelocityFromPos: setMeshVelocityFromPos,
		setMeshGravity: setMeshGravity,
		sphericalJoinLink: sphericalJoinLink,
		fixedJoinLink: fixedJoinLink,
		getMeshRotation: getMeshRotation,
	};

}

export { RapierPhysics };
