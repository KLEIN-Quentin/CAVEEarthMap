import * as THREE from 'three';
import { RapierPhysics } from './RapierPhysics.js';

const ropeRestitution = 1;
const ropeFriction = 1;
const ropeLinearDamping = 5.0;
const ropeAngularDamping = 5.0;
const ropeSphereRadius = 0.05;
const ropeSphereMass = 0.5;

const ropeSphereGeometry = new THREE.IcosahedronGeometry(ropeSphereRadius, 3);

const ropeMinisphereRadius = 0.04;
const ropeMinipshereMass = 0.2;

const ropeMinisphereGeometry = new THREE.IcosahedronGeometry(ropeMinisphereRadius, 3);

const ropeMaterial = new THREE.MeshNormalMaterial( { visible: true } );

const boardMass = 1.25;
const boardFriction = 1;
const boardRestitution = 1;
const boardLinearDamping = 2.0;

const weightMass = 0.5;

const cubeGeometry = new THREE.BoxGeometry(0.12, 0.12, 0.12);
const cubeMaterial = new THREE.MeshNormalMaterial( { visible: true } );

const cubeMass = 0.02;
const cubeFriction = 0.8;


export class PhysicsEngine {
    #physics;

    #scene;
    #rope_sphere;
    #rope_minisphere;
    #board;
    #cubes;

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

        this.#rope_sphere = [];
        this.#rope_minisphere = [];
        this.#cubes = [];

        const floorGeometry = new THREE.BoxGeometry(20, 2, 30);
        const floorMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const floor = new THREE.Mesh(floorGeometry, floorMaterial);
        floor.position.copy(new THREE.Vector3(0, -3, -6));
        floor.userData.physics = {mass: 0};
        this.#scene.add(floor);

        const wallGeometry1 = new THREE.BoxGeometry(2, 20, 20);
        const wallMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const wall1 = new THREE.Mesh(wallGeometry1, wallMaterial);
        wall1.position.copy(new THREE.Vector3(-2.3, -5, -6));
        wall1.userData.physics = {mass: 0};
        this.#scene.add(wall1);
        const wall2 = new THREE.Mesh(wallGeometry1, wallMaterial);
        wall2.position.copy(new THREE.Vector3(2.3, -5, -6));
        wall2.userData.physics = {mass: 0};
        this.#scene.add(wall2);

        const scaffoldGeometry = new THREE.BoxGeometry(0.8, 0.2, 4);
        const scaffoldMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const scaffold1 = new THREE.Mesh(scaffoldGeometry, scaffoldMaterial);
        scaffold1.position.copy(new THREE.Vector3(0.9, -0.23, -6));
        scaffold1.userData.physics = {mass: 0};
        this.#scene.add(scaffold1);
        const scaffold2 = new THREE.Mesh(scaffoldGeometry, scaffoldMaterial);
        scaffold2.position.copy(new THREE.Vector3(-0.9, -0.23, -6));
        scaffold2.userData.physics = {mass: 0};
        this.#scene.add(scaffold2);

        const scaffoldFootGeometry = new THREE.BoxGeometry(0.8, 15, 0.3);
        const scaffoldFootMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const scaffoldFoot1 = new THREE.Mesh(scaffoldFootGeometry, scaffoldFootMaterial);
        scaffoldFoot1.position.copy(new THREE.Vector3(-0.9, -7.8, -7.5));
        scaffoldFoot1.userData.physics = {mass: 0};
        this.#scene.add(scaffoldFoot1);
        const scaffoldFoot2 = new THREE.Mesh(scaffoldFootGeometry, scaffoldFootMaterial);
        scaffoldFoot2.position.copy(new THREE.Vector3(0.9, -7.8, -7.5));
        scaffoldFoot2.userData.physics = {mass: 0};
        this.#scene.add(scaffoldFoot2);
        const scaffoldFoot3 = new THREE.Mesh(scaffoldFootGeometry, scaffoldFootMaterial);
        scaffoldFoot3.position.copy(new THREE.Vector3(-0.9, -7.8, -4.5));
        scaffoldFoot3.userData.physics = {mass: 0};
        this.#scene.add(scaffoldFoot3);
        const scaffoldFoot4 = new THREE.Mesh(scaffoldFootGeometry, scaffoldFootMaterial);
        scaffoldFoot4.position.copy(new THREE.Vector3(0.9, -7.8, -4.5));
        scaffoldFoot4.userData.physics = {mass: 0};
        this.#scene.add(scaffoldFoot4);

        const scaffoldGatewayGeometry = new THREE.BoxGeometry(1.2, 0.1, 0.7);
        const scaffoldGatewayMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        const scaffoldGateway1 = new THREE.Mesh(scaffoldGatewayGeometry, scaffoldGatewayMaterial);
        scaffoldGateway1.position.copy(new THREE.Vector3(0, -0.18, -7));
        scaffoldGateway1.userData.physics = {mass: 0};
        this.#scene.add(scaffoldGateway1);
        const scaffoldGateway2 = new THREE.Mesh(scaffoldGatewayGeometry, scaffoldGatewayMaterial);
        scaffoldGateway2.position.copy(new THREE.Vector3(0, -0.18, -5));
        scaffoldGateway2.userData.physics = {mass: 0};
        this.#scene.add(scaffoldGateway2);

        const boardGeometry = new THREE.BoxGeometry(0.7, 0.12, 0.5)
        const boardMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        this.#board = new THREE.Mesh(boardGeometry, boardMaterial);
        this.#board.position.copy(new THREE.Vector3(0, -0.05, -5));
        this.#board.userData.physics = {mass: boardMass, friction: boardFriction, restitution: boardRestitution, linearDamping: boardLinearDamping};
        this.#scene.add(this.#board);

        this.#physics.addScene(this.#scene);

        // this.#physics.sphericalJoinLink(this.#rope_sphere[0], this.#rope_minisphere[0], new THREE.Vector3(-0.052, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[0], this.#rope_minisphere[1], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[1], this.#rope_minisphere[2], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[2], this.#rope_minisphere[3], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[3], this.#rope_minisphere[4], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[4], this.#rope_minisphere[5], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[5], this.#rope_minisphere[6], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));

        // this.#physics.sphericalJoinLink(this.#rope_sphere[1], this.#rope_minisphere[7], new THREE.Vector3(-0.052, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[7], this.#rope_minisphere[8], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[8], this.#rope_minisphere[9], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[9], this.#rope_minisphere[10], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[10], this.#rope_minisphere[11], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[11], this.#rope_minisphere[12], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[12], this.#rope_minisphere[13], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));

        // this.#physics.sphericalJoinLink(this.#rope_sphere[2], this.#rope_minisphere[14], new THREE.Vector3(0.052, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[14], this.#rope_minisphere[15], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[15], this.#rope_minisphere[16], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[16], this.#rope_minisphere[17], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[17], this.#rope_minisphere[18], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[18], this.#rope_minisphere[19], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[19], this.#rope_minisphere[20], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));

        // this.#physics.sphericalJoinLink(this.#rope_sphere[3], this.#rope_minisphere[21], new THREE.Vector3(0.052, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[21], this.#rope_minisphere[22], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[22], this.#rope_minisphere[23], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[23], this.#rope_minisphere[24], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[24], this.#rope_minisphere[25], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[25], this.#rope_minisphere[26], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[26], this.#rope_minisphere[27], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));

        // this.#physics.sphericalJoinLink(this.#rope_minisphere[6], this.#board, new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.37, -0.0, -0.25));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[13], this.#board, new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.37, -0.0, 0.25));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[20], this.#board, new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.37, -0.0, -0.25));
        // this.#physics.sphericalJoinLink(this.#rope_minisphere[27], this.#board, new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.37, -0.0, 0.25));
    }

    get scene ( ) {
        return this.#scene;
    }

    addRope(position, id, xOffset) {
        if (id >= 0 && id < 4)
        {
            const tmp_scene = new THREE.Scene();
            if (xOffset < 0)
            {
                this.#rope_sphere[id]  = new THREE.Mesh(ropeSphereGeometry, ropeMaterial);
                this.#rope_sphere[id].position.copy(position);
                this.#rope_sphere[id].userData.physics = {mass: ropeSphereMass, restitution: ropeRestitution, linearDamping: ropeLinearDamping, angularDamping: ropeAngularDamping, friction: ropeFriction};
                tmp_scene.add(this.#rope_sphere[id]);
                for (let i = 0; i < 7; i++)
                {
                    this.#rope_minisphere[id * 7 + i] = new THREE.Mesh(ropeMinisphereGeometry, ropeMaterial);
                    this.#rope_minisphere[id * 7 + i].position.copy(new THREE.Vector3(position.x + xOffset, position.y, position.z));
                    this.#rope_minisphere[id * 7 + i].userData.physics = {mass: ropeMinipshereMass, restitution: ropeRestitution, linearDamping: ropeLinearDamping, angularDamping: ropeAngularDamping, friction: ropeFriction};
                    tmp_scene.add(this.#rope_minisphere[id * 7 + i]);
                    xOffset -= 0.075;
                }
            }
            else
            {
                this.#rope_sphere[id]  = new THREE.Mesh(ropeSphereGeometry, ropeMaterial);
                this.#rope_sphere[id].position.copy(position);
                this.#rope_sphere[id].userData.physics = {mass: ropeSphereMass, restitution: ropeRestitution, linearDamping: ropeLinearDamping, angularDamping: ropeAngularDamping, friction: ropeFriction};
                tmp_scene.add(this.#rope_sphere[id]);
                for (let i = 0; i < 7; i++)
                {
                    this.#rope_minisphere[id * 7 + i] = new THREE.Mesh(ropeMinisphereGeometry, ropeMaterial);
                    this.#rope_minisphere[id * 7 + i].position.copy(new THREE.Vector3(position.x + xOffset, position.y, position.z));
                    this.#rope_minisphere[id * 7 + i].userData.physics = {mass: ropeMinipshereMass, restitution: ropeRestitution, linearDamping: ropeLinearDamping, angularDamping: ropeAngularDamping, friction: ropeFriction};
                    tmp_scene.add(this.#rope_minisphere[id * 7 + i]);
                    xOffset += 0.075;
                }
            }
            this.#physics.addScene(tmp_scene);
        }
        else
        {
            console.log("id incorrect !");
        }
    }

    manageLevel(lvl_number) {
        const tmp_scene = new THREE.Scene();
        const obstacleGeometry = new THREE.BoxGeometry(1, 0.6, 0.2);
        const obstacleMaterial = new THREE.MeshNormalMaterial( { visible: true } );
        if (lvl_number >= 1)
        {
            const obstacle1 = new THREE.Mesh(obstacleGeometry, obstacleMaterial);
            obstacle1.position.copy(new THREE.Vector3(0, 0, -6));
            obstacle1.userData.physics = {mass: 0};
            tmp_scene.add(obstacle1);
        }
        if (lvl_number >= 2)
        {
            const obstacle2 = new THREE.Mesh(obstacleGeometry, obstacleMaterial);
            obstacle2.position.copy(new THREE.Vector3(0, 1.15, -6));
            obstacle2.userData.physics = {mass: 0};
            tmp_scene.add(obstacle2);
        }
        if (lvl_number >= 3)
        {
            const obstacle3 = new THREE.Mesh(obstacleGeometry, obstacleMaterial);
            obstacle3.position.copy(new THREE.Vector3(0, 0.8, -5.2));
            obstacle3.userData.physics = {mass: 0};
            tmp_scene.add(obstacle3);
        }
        if (lvl_number >= 4)
        {
            const obstacle4 = new THREE.Mesh(obstacleGeometry, obstacleMaterial);
            obstacle4.position.copy(new THREE.Vector3(0, 0.8, -6.8));
            obstacle4.userData.physics = {mass: 0};
            tmp_scene.add(obstacle4);
        }
        this.#physics.addScene(tmp_scene);
    }

    createRopes (ropeNumber) {
        if (ropeNumber == 2) 
        {
            const tmp_scene = new THREE.Scene();

            const weightGeometry = new THREE.BoxGeometry(0.01, 0.01, 0.01);
            const weightMaterial = new THREE.MeshNormalMaterial( { visible: true } );
            const weight1 = new THREE.Mesh(weightGeometry, weightMaterial);
            weight1.position.copy(new THREE.Vector3(0,-0.105,-5.255));
            weight1.userData.physics = {mass: weightMass};
            tmp_scene.add(weight1);
            const weight2 = new THREE.Mesh(weightGeometry, weightMaterial);
            weight2.position.copy(new THREE.Vector3(0,-0.105,-4.745));
            weight2.userData.physics = {mass: weightMass};
            tmp_scene.add(weight2);

            this.#physics.addScene(tmp_scene);

            this.#physics.fixedJoinLink(this.#board, weight1, new THREE.Vector3(0, -0.055, -0.25), new THREE.Vector3(0, 0, 0.005));
            this.#physics.fixedJoinLink(this.#board, weight2, new THREE.Vector3(0, -0.055, 0.25), new THREE.Vector3(0, 0, -0.005));

            this.addRope(new THREE.Vector3(0.93, -0.05, -5), 0, -0.085);
            this.addRope(new THREE.Vector3(-0.93, -0.05, -5), 1, 0.085);

            this.#physics.sphericalJoinLink(this.#rope_sphere[0], this.#rope_minisphere[0], new THREE.Vector3(-0.052, 0, 0), new THREE.Vector3(0.042,0,0));
            for (let j = 0; j < 6; j++)
            {
                this.#physics.sphericalJoinLink(this.#rope_minisphere[j], this.#rope_minisphere[j + 1], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
            }
            this.#physics.sphericalJoinLink(this.#rope_minisphere[6], this.#board, new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.37, -0.0, 0));

            this.#physics.sphericalJoinLink(this.#rope_sphere[1], this.#rope_minisphere[7], new THREE.Vector3(0.052, 0, 0), new THREE.Vector3(-0.042,0,0));
            for (let j = 0; j < 6; j++)
            {
                this.#physics.sphericalJoinLink(this.#rope_minisphere[7 + j], this.#rope_minisphere[7 + j + 1], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
            }
            this.#physics.sphericalJoinLink(this.#rope_minisphere[13], this.#board, new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.37, -0.0, 0));
        }
        else if (ropeNumber == 4)
        {
            this.addRope(new THREE.Vector3(0.93, -0.05, -5.25), 0, -0.085);
            this.addRope(new THREE.Vector3(0.93, -0.05, -4.75), 1, -0.085);
            this.addRope(new THREE.Vector3(-0.93, -0.05, -5.25), 2, 0.085);
            this.addRope(new THREE.Vector3(-0.93, -0.05, -4.75), 3, 0.085);

            for (let i = 0; i < 2; i++)
            {
                this.#physics.sphericalJoinLink(this.#rope_sphere[i], this.#rope_minisphere[i*7], new THREE.Vector3(-0.052, 0, 0), new THREE.Vector3(0.042,0,0));
                for (let j = 0; j < 6; j++)
                {
                    this.#physics.sphericalJoinLink(this.#rope_minisphere[i*7 + j], this.#rope_minisphere[i*7 + j + 1], new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.042,0,0));
                }
            }

            for (let i = 2; i < 4; i++)
            {
                this.#physics.sphericalJoinLink(this.#rope_sphere[i], this.#rope_minisphere[i*7], new THREE.Vector3(0.052, 0, 0), new THREE.Vector3(-0.042,0,0));
                for (let j = 0; j < 6; j++)
                {
                    this.#physics.sphericalJoinLink(this.#rope_minisphere[i*7 + j], this.#rope_minisphere[i*7 + j + 1], new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.042,0,0));
                }
            }

            this.#physics.sphericalJoinLink(this.#rope_minisphere[6], this.#board, new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.37, -0.0, -0.25));
            this.#physics.sphericalJoinLink(this.#rope_minisphere[13], this.#board, new THREE.Vector3(-0.042, 0, 0), new THREE.Vector3(0.37, -0.0, 0.25));
            this.#physics.sphericalJoinLink(this.#rope_minisphere[20], this.#board, new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.37, -0.0, -0.25));
            this.#physics.sphericalJoinLink(this.#rope_minisphere[27], this.#board, new THREE.Vector3(0.042, 0, 0), new THREE.Vector3(-0.37, -0.0, 0.25));
        }
        else 
        {
            console.log("pas de situation pour ce nombre de cordes !");
        }
    }

    addCube(position, id)
    {
        const tmp_scene = new THREE.Scene();
        this.#cubes[id] = new THREE.Mesh(cubeGeometry, cubeMaterial);
        this.#cubes[id].position.copy(position);
        this.#cubes[id].userData.physics = {mass: cubeMass, friction: cubeFriction};
        tmp_scene.add(this.#cubes[id]);
        this.#physics.addScene(tmp_scene);
    }

    setCube(position, rotation, id)
    {
        this.#physics.setMeshPosition(this.#cubes[id], position);
        this.#physics.setMeshRotation(this.#cubes[id], rotation);
        this.#physics.setMeshVelocity(this.#cubes[id], new THREE.Vector3(0,0,0));
    }

    reset(ropeNumber)
    {
        this.#physics.setMeshPosition(this.#board, new THREE.Vector3(0, -0.05, -5));
        this.#physics.setMeshRotation(this.#board, new THREE.Quaternion(0,0,0,1));
        this.#physics.setMeshVelocity(this.#board, new THREE.Vector3(0,0,0));

        if (ropeNumber == 2) 
        {
            let xOffset = -0.085
            this.#physics.setMeshPosition(this.#rope_sphere[0], new THREE.Vector3(0.93, -0.05, -5));
            this.#physics.setMeshRotation(this.#rope_sphere[0], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[0], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[i], new THREE.Vector3(0.93 + xOffset, -0.05, -5));
                this.#physics.setMeshRotation(this.#rope_minisphere[i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[i], new THREE.Vector3(0,0,0));
                xOffset -= 0.075;
            }
            xOffset = 0.085
            this.#physics.setMeshPosition(this.#rope_sphere[1], new THREE.Vector3(-0.93, -0.05, -5));
            this.#physics.setMeshRotation(this.#rope_sphere[1], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[1], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[7 + i], new THREE.Vector3(-0.93 + xOffset, -0.05, -5));
                this.#physics.setMeshRotation(this.#rope_minisphere[7 + i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[7 + i], new THREE.Vector3(0,0,0));
                xOffset += 0.075;
            }
        }
        else if (ropeNumber == 4)
        {
            let xOffset = -0.085
            this.#physics.setMeshPosition(this.#rope_sphere[0], new THREE.Vector3(0.93, -0.05, -5.25));
            this.#physics.setMeshRotation(this.#rope_sphere[0], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[0], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[i], new THREE.Vector3(0.93 + xOffset, -0.05, -5.25));
                this.#physics.setMeshRotation(this.#rope_minisphere[i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[i], new THREE.Vector3(0,0,0));
                xOffset -= 0.075;
            }
            xOffset = -0.085
            this.#physics.setMeshPosition(this.#rope_sphere[1], new THREE.Vector3(0.93, -0.05, -4.75));
            this.#physics.setMeshRotation(this.#rope_sphere[1], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[1], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[7 + i], new THREE.Vector3(0.93 + xOffset, -0.05, -4.75));
                this.#physics.setMeshRotation(this.#rope_minisphere[7 + i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[7 + i], new THREE.Vector3(0,0,0));
                xOffset -= 0.075;
            }
            xOffset = 0.085
            this.#physics.setMeshPosition(this.#rope_sphere[2], new THREE.Vector3(-0.93, -0.05, -5.25));
            this.#physics.setMeshRotation(this.#rope_sphere[2], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[2], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[14 + i], new THREE.Vector3(-0.93 + xOffset, -0.05, -5.25));
                this.#physics.setMeshRotation(this.#rope_minisphere[14 + i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[14 + i], new THREE.Vector3(0,0,0));
                xOffset += 0.075;
            }
            xOffset = 0.085
            this.#physics.setMeshPosition(this.#rope_sphere[3], new THREE.Vector3(-0.93, -0.05, -4.75));
            this.#physics.setMeshRotation(this.#rope_sphere[3], new THREE.Quaternion(0,0,0,1));
            this.#physics.setMeshVelocity(this.#rope_sphere[3], new THREE.Vector3(0,0,0));
            for (let i = 0; i < 7; i++)
            {
                this.#physics.setMeshPosition(this.#rope_minisphere[21 + i], new THREE.Vector3(-0.93 + xOffset, -0.05, -4.75));
                this.#physics.setMeshRotation(this.#rope_minisphere[21 + i], new THREE.Quaternion(0,0,0,1));
                this.#physics.setMeshVelocity(this.#rope_minisphere[21 + i], new THREE.Vector3(0,0,0));
                xOffset += 0.075;
            }
        }
    }

    setRope ( position, id, velocity ) {
        this.#physics.setMeshPosition(this.#rope_sphere[id], position);
        if( velocity ){
            // this.#physics.setMeshPosition(this.#ball, new THREE.Vector3(-2, 2.23, -5.627565));
            // console.log("release ball", velocity)
            this.#physics.setMeshVelocity(this.#rope_sphere[id], velocity);

        }
    }

    setRopeVelocity (velocity, id) {
        this.#physics.setMeshVelocity(this.#rope_sphere[id], velocity);
    }

    setRopeGravity (gravity, id) {
        this.#physics.setMeshGravity(this.#rope_sphere[id], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+1], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+2], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+3], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+4], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+5], gravity);
        this.#physics.setMeshGravity(this.#rope_minisphere[7*id+6], gravity);
    }

    getRopeSpherePosition(id) {
        return this.#rope_sphere[id].position;
    }

    getRopeMinispherePosition(id, part) {
        return this.#rope_minisphere[id * 7 + part].position;
    }

    getBoardPosition() {
        return this.#board.position;
    }

    getBoardRotation() {
        return this.#physics.getMeshRotation(this.#board);
    }

    getCubePosition(id) {
        return this.#cubes[id].position;
    }

    getCubeRotation(id) {
        return this.#physics.getMeshRotation(this.#cubes[id]);
    }

    removeRopes() {
        this.#rope_sphere.forEach(ball => {
            this.#physics.removeMesh(ball);
        });
    }

    releaseRope ( pos, pos_1, id) {
        console.log("release ball")
        this.#physics.setMeshVelocityFromPos(this.#rope_sphere[id], pos, pos_1);
    }

    step ( ) {
        this.#physics.step();
    }

    start ( ) {
        this.#physics.start();
    }
}