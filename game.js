class TapdownGame {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.scoreElement = document.getElementById('score');
        
        this.gameState = 'stopped'; // 'stopped', 'running', 'paused'
        this.score = 0;
        this.fallingObjects = [];
        this.particles = [];
        this.lastTime = 0;
        
        this.objectSpawnRate = 2000; // milliseconds
        this.lastSpawn = 0;
        
        this.init();
    }
    
    init() {
        this.setupEventListeners();
        this.gameLoop();
    }
    
    setupEventListeners() {
        // Canvas click/tap events
        this.canvas.addEventListener('click', (e) => this.handleTap(e));
        this.canvas.addEventListener('touchstart', (e) => {
            e.preventDefault();
            this.handleTap(e.touches[0]);
        });
        
        // Control buttons
        document.getElementById('startBtn').addEventListener('click', () => this.startGame());
        document.getElementById('pauseBtn').addEventListener('click', () => this.togglePause());
        document.getElementById('resetBtn').addEventListener('click', () => this.resetGame());
    }
    
    handleTap(e) {
        if (this.gameState !== 'running') return;
        
        const rect = this.canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        
        // Check if tap hit any falling object
        for (let i = this.fallingObjects.length - 1; i >= 0; i--) {
            const obj = this.fallingObjects[i];
            const distance = Math.sqrt((x - obj.x) ** 2 + (y - obj.y) ** 2);
            
            if (distance < obj.radius) {
                // Hit! Ignite the object
                this.igniteObject(obj, i);
                break;
            }
        }
    }
    
    igniteObject(obj, index) {
        // Remove the object
        this.fallingObjects.splice(index, 1);
        
        // Add score
        this.score += 10;
        this.updateScore();
        
        // Create particle explosion
        this.createExplosion(obj.x, obj.y);
        
        // Increase difficulty slightly
        if (this.objectSpawnRate > 800) {
            this.objectSpawnRate -= 10;
        }
    }
    
    createExplosion(x, y) {
        for (let i = 0; i < 15; i++) {
            this.particles.push({
                x: x,
                y: y,
                vx: (Math.random() - 0.5) * 10,
                vy: (Math.random() - 0.5) * 10,
                life: 1.0,
                decay: Math.random() * 0.02 + 0.02,
                size: Math.random() * 4 + 2,
                color: `hsl(${Math.random() * 60 + 10}, 100%, 60%)`
            });
        }
    }
    
    spawnFallingObject(currentTime) {
        if (currentTime - this.lastSpawn > this.objectSpawnRate) {
            this.fallingObjects.push({
                x: Math.random() * (this.canvas.width - 60) + 30,
                y: -30,
                radius: 15 + Math.random() * 10,
                speed: 2 + Math.random() * 3,
                color: `hsl(${Math.random() * 360}, 70%, 50%)`,
                rotation: 0,
                rotationSpeed: (Math.random() - 0.5) * 0.2
            });
            this.lastSpawn = currentTime;
        }
    }
    
    update(deltaTime) {
        if (this.gameState !== 'running') return;
        
        // Update falling objects
        for (let i = this.fallingObjects.length - 1; i >= 0; i--) {
            const obj = this.fallingObjects[i];
            obj.y += obj.speed;
            obj.rotation += obj.rotationSpeed;
            
            // Remove objects that fell off screen
            if (obj.y > this.canvas.height + 50) {
                this.fallingObjects.splice(i, 1);
            }
        }
        
        // Update particles
        for (let i = this.particles.length - 1; i >= 0; i--) {
            const particle = this.particles[i];
            particle.x += particle.vx;
            particle.y += particle.vy;
            particle.life -= particle.decay;
            particle.vy += 0.2; // gravity
            
            if (particle.life <= 0) {
                this.particles.splice(i, 1);
            }
        }
        
        // Spawn new objects
        this.spawnFallingObject(Date.now());
    }
    
    render() {
        // Clear canvas
        this.ctx.fillStyle = '#000011';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw falling objects
        this.fallingObjects.forEach(obj => {
            this.ctx.save();
            this.ctx.translate(obj.x, obj.y);
            this.ctx.rotate(obj.rotation);
            
            // Draw object with glow effect
            this.ctx.shadowColor = obj.color;
            this.ctx.shadowBlur = 20;
            this.ctx.fillStyle = obj.color;
            this.ctx.beginPath();
            this.ctx.arc(0, 0, obj.radius, 0, Math.PI * 2);
            this.ctx.fill();
            
            // Draw inner core
            this.ctx.shadowBlur = 0;
            this.ctx.fillStyle = '#fff';
            this.ctx.beginPath();
            this.ctx.arc(0, 0, obj.radius * 0.3, 0, Math.PI * 2);
            this.ctx.fill();
            
            this.ctx.restore();
        });
        
        // Draw particles
        this.particles.forEach(particle => {
            this.ctx.globalAlpha = particle.life;
            this.ctx.fillStyle = particle.color;
            this.ctx.beginPath();
            this.ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
            this.ctx.fill();
        });
        this.ctx.globalAlpha = 1;
        
        // Draw game state text
        if (this.gameState === 'stopped') {
            this.ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
            this.ctx.font = '48px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('Click Start to Play!', this.canvas.width / 2, this.canvas.height / 2);
        } else if (this.gameState === 'paused') {
            this.ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
            this.ctx.font = '48px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('PAUSED', this.canvas.width / 2, this.canvas.height / 2);
        }
    }
    
    gameLoop(currentTime = 0) {
        const deltaTime = currentTime - this.lastTime;
        this.lastTime = currentTime;
        
        this.update(deltaTime);
        this.render();
        
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    startGame() {
        if (this.gameState === 'stopped') {
            this.resetGame();
        }
        this.gameState = 'running';
        this.lastSpawn = Date.now();
        document.getElementById('startBtn').textContent = 'Resume';
    }
    
    togglePause() {
        if (this.gameState === 'running') {
            this.gameState = 'paused';
        } else if (this.gameState === 'paused') {
            this.gameState = 'running';
        }
    }
    
    resetGame() {
        this.gameState = 'stopped';
        this.score = 0;
        this.fallingObjects = [];
        this.particles = [];
        this.objectSpawnRate = 2000;
        this.updateScore();
        document.getElementById('startBtn').textContent = 'Start Game';
    }
    
    updateScore() {
        this.scoreElement.textContent = this.score;
    }
}

// Initialize the game when the page loads
window.addEventListener('load', () => {
    new TapdownGame();
});