import * as THREE from 'three';
import { RapierPhysics } from './RapierPhysics.js';

const tableDims = {
    length: 2.74,
    height: 2,
    width: 1.525,
}
const petanqueBallRadius = 0.037;
const petanqueBallBounce = 0.2;
const petanqueBallMass = 0.71;
const petanqueBallFriction = 1.3;
const petanqueBallLinearDampling = 0.6;
const petanqueBallAngularDampling = 1.5;

const butRadius = 0.015;
const butBounce = 0.25;
const butMass = 0.65;
const butFriction = 1;
const butLinearDampling = 0.5;
const butAngularDampling = 1.25;

const petanqueBallGeometry = new THREE.IcosahedronGeometry(petanqueBallRadius, 3);
const petanqueBallMaterial = new THREE.MeshNormalMaterial( { visible: true } );

const butGeometry = new THREE.IcosahedronGeometry(butRadius, 3);
const butMaterial = new THREE.MeshNormalMaterial( { visible: true } );


export class PhysicsEngine {
    #physics;

    #scene;
    #paddles;
    #balls;

    time = 0;

    constructor ( physics ) {
        this.#physics = physics;
    }

    static async initialize ( ) {
        const physics = await RapierPhysics();
        const engine = new PhysicsEngine( physics );
        return engine;
    }

    createScene ( ) {
        this.#scene = new THREE.Scene();

        this.#balls = [];

        const floorGeometry = new THREE.BoxGeometry(24, 2, 12);
        const floorMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const floor = new THREE.Mesh(floorGeometry, floorMaterial);
        floor.position.copy(new THREE.Vector3(1.57, -1, -5.65));
        floor.userData.physics = {mass: 0};
        this.#scene.add(floor);

        const playgroundGeometry = new THREE.BoxGeometry(15, 2, 4);
        const playgroundMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const playground = new THREE.Mesh(playgroundGeometry, playgroundMaterial);
        playground.position.copy(new THREE.Vector3(2.4, -0.99, -5.65));
        playground.userData.physics = {mass: 0};
        this.#scene.add(playground);

        const wallGeometry1 = new THREE.BoxGeometry(2, 13, 12);
        const wallGeometry2 = new THREE.BoxGeometry(24, 13, 2);
        const wallMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const wall1 = new THREE.Mesh(wallGeometry1, wallMaterial);
        wall1.position.copy(new THREE.Vector3(1.57 + 11, 4.5, -5.65));
        wall1.userData.physics = {mass: 0};
        this.#scene.add(wall1);
        const wall2 = new THREE.Mesh(wallGeometry1, wallMaterial);
        wall2.position.copy(new THREE.Vector3(1.57 - 11, 4.5, -5.65));
        wall2.userData.physics = {mass: 0};
        this.#scene.add(wall2);
        const wall3 = new THREE.Mesh(wallGeometry2, wallMaterial);
        wall3.position.copy(new THREE.Vector3(1.57, 4.5, -0.65));
        wall3.userData.physics = {mass: 0};
        this.#scene.add(wall3);
        const wall4 = new THREE.Mesh(wallGeometry2, wallMaterial);
        wall4.position.copy(new THREE.Vector3(1.57, 4.5, -10.65));
        wall4.userData.physics = {mass: 0};
        this.#scene.add(wall4);

        const roofGeometry = new THREE.BoxGeometry(24, 2, 12);
        const roofMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const roof = new THREE.Mesh(roofGeometry, roofMaterial);
        roof.position.copy(new THREE.Vector3(1.57, 9.75, -5.65));
        roof.userData.physics = {mass: 0};
        this.#scene.add(roof);

        const tableGeometry = new THREE.BoxGeometry(1.59, 0.3, 0.455);
        const tableMaterial = new THREE.MeshNormalMaterial( { visible: true} );
        const table = new THREE.Mesh(tableGeometry, tableMaterial);
        table.position.copy(new THREE.Vector3(-4.04273, 0.675, -4.687667));
        table.userData.physics = {mass: 0};
        this.#scene.add(table);

        const tableFeetGeometry = new THREE.BoxGeometry(0.042, 0.825, 0.365);
        const tableFeetMaterial = new THREE.MeshNormalMaterial( { visible: true} );
        const tableFeet1 = new THREE.Mesh(tableFeetGeometry, tableFeetMaterial);
        tableFeet1.position.copy(new THREE.Vector3(-4.601, 0.414, -4.687667));
        tableFeet1.userData.physics = {mass: 0};
        this.#scene.add(tableFeet1);
        const tableFeet2 = new THREE.Mesh(tableFeetGeometry, tableFeetMaterial);
        tableFeet2.position.copy(new THREE.Vector3(-3.476, 0.414, -4.687667));
        tableFeet2.userData.physics = {mass: 0};
        this.#scene.add(tableFeet2);

        const panelGeometry = new THREE.BoxGeometry(1, 1, 0.1);
        const panelMaterial = new THREE.MeshNormalMaterial( { visible: true} );
        const panel = new THREE.Mesh(panelGeometry, panelMaterial);
        panel.position.copy(new THREE.Vector3(-2.561, 0.677, -7.721));
        panel.userData.physics = {mass: 0};
        this.#scene.add(panel);

        const mapGeometry = new THREE.BoxGeometry(2, 2, 0.1);
        const mapMaterial = new THREE.MeshNormalMaterial( { visible: true} );
        const map = new THREE.Mesh(mapGeometry, mapMaterial);
        map.position.copy(new THREE.Vector3(-4.049, 1.177, -7.721));
        map.userData.physics = {mass: 0};
        this.#scene.add(map);

        const boardsWidthGeometry = new THREE.BoxGeometry(0.15, 0.2, 4.3);
        const boardsWidthMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const frontBoard = new THREE.Mesh(boardsWidthGeometry, boardsWidthMaterial);
        frontBoard.position.copy(new THREE.Vector3(2.4 - 7.575, 0.1, -5.65));
        frontBoard.userData.physics = {mass: 0};
        this.#scene.add(frontBoard);
        const backBoard = new THREE.Mesh(boardsWidthGeometry, boardsWidthMaterial);
        backBoard.position.copy(new THREE.Vector3(2.4 + 7.575, 0.1, -5.65));
        backBoard.userData.physics = {mass: 0};
        this.#scene.add(backBoard);

        const boardsLengthGeometry = new THREE.BoxGeometry(15.3, 0.2, 0.15);
        const boardsLengthMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const leftBoard = new THREE.Mesh(boardsLengthGeometry, boardsLengthMaterial);
        leftBoard.position.copy(new THREE.Vector3(2.4, 0.1, -5.65 + 2.075));
        leftBoard.userData.physics = {mass: 0};
        this.#scene.add(leftBoard);
        const rightBoard = new THREE.Mesh(boardsLengthGeometry, boardsLengthMaterial);
        rightBoard.position.copy(new THREE.Vector3(2.4, 0.1, -5.65 - 2.075));
        rightBoard.userData.physics = {mass: 0};
        this.#scene.add(rightBoard);

        this.#physics.addScene(this.#scene);
    }

    get scene ( ) {
        return this.#scene;
    }

    addBall (position, id) {
        const tmp_scene = new THREE.Scene();
        if (id > 0)
        {
            this.#balls[id] = new THREE.Mesh(petanqueBallGeometry, petanqueBallMaterial);
            this.#balls[id].position.copy(position);
            this.#balls[id].userData.physics = {mass: petanqueBallMass, restitution: petanqueBallBounce, friction: petanqueBallFriction, linearDampling: petanqueBallLinearDampling, angularDampling: petanqueBallAngularDampling};
            tmp_scene.add(this.#balls[id]);
        }
        else
        {
            this.#balls[id] = new THREE.Mesh(butGeometry, butMaterial);
            this.#balls[id].position.copy(position);
            this.#balls[id].userData.physics = {mass: butMass, restitution: butBounce, friction: butFriction, linearDampling: butLinearDampling, angularDampling: butAngularDampling};
            tmp_scene.add(this.#balls[id]);
        }
        this.#physics.addScene(tmp_scene);
    }

    setBall ( position, id, velocity ) {
        this.#physics.setMeshPosition(this.#balls[id], position);
        if( velocity ){
            // this.#physics.setMeshPosition(this.#ball, new THREE.Vector3(-2, 2.23, -5.627565));
            // console.log("release ball", velocity)
            this.#physics.setMeshVelocity(this.#balls[id], velocity);

        }
    }

    setBallVelocity (velocity, id) {
        this.#physics.setMeshVelocity(this.#balls[id], velocity);
    }

    getBallPosition(id) {
        return this.#balls[id].position;
    }

    removeBalls() {
        this.#balls.forEach(ball => {
            this.#physics.removeMesh(ball);
        });
    }

    releaseBall ( pos, pos_1, id) {
        console.log("release ball")
        this.#physics.setMeshVelocityFromPos(this.#balls[id], pos, pos_1);
    }

    setPaddle ( id, position, quaternion ) {
        this.#paddles[id].position.copy(position);
        this.#paddles[id].quaternion.copy(quaternion);
    }

    step ( ) {
        this.#physics.step();
    }

    start ( ) {
        this.#physics.start();
    }
}