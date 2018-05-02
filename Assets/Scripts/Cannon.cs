using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct ProjectileInfo {
    public GameObject projectile;
    public float timeCreated;

    public ProjectileInfo(GameObject obj, float time) {
        this.projectile  = obj;
        this.timeCreated = time;
    }
};

public class Cannon: MonoBehaviour {

    private float forceMult = 40.0f;

    private bool _enabled = false; 

    private float      angle = 0.0f;  
    private Vector3    originalPosition;
    private Quaternion originalRotation;

    public int ProjectileLayer;
    public int Health;
    public Text ScoreDisplay = null;


    private readonly float fireCooldownTime = 1.0f;
    private float elapsedTimeSinceLastFire;


    private float projectileLifetime = 10.0f;


    private Queue<ProjectileInfo> projectiles;


    // reference to global gameManager object
    private GameObject gameManager;

	void Start () {
        this.originalPosition = this.gameObject.transform.position;
        this.originalRotation = this.gameObject.transform.rotation;
        this.elapsedTimeSinceLastFire = this.fireCooldownTime;
        this.projectiles = new Queue<ProjectileInfo>();
        this.gameManager = GameObject.Find("GameManager");

        if (this.ScoreDisplay) {
            this.ScoreDisplay.text = "" + this.Health;
        }

        
        Physics.IgnoreLayerCollision(this.gameObject.layer, this.ProjectileLayer, true);

	}
	
	// Update is called once per frame
	void Update () {
        this.CleanupProjectiles();
        this.gameObject.transform.position = this.originalPosition;
        this.gameObject.transform.rotation = this.originalRotation;

        float rot_step = 0.2f;
        if (Input.GetKey(KeyCode.LeftArrow)  && this._enabled)   this.angle -= rot_step*Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow) && this._enabled)  this.angle += rot_step*Time.deltaTime;


        gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Mathf.Rad2Deg * this.angle);
        if (Input.GetKey(KeyCode.Space) && CanFire()) {
            this.ShootProjectile();
            this.elapsedTimeSinceLastFire = 0.0f;
            this.gameManager.SendMessage("OnProjectileFired", this.gameObject.name);
        }

        elapsedTimeSinceLastFire += Time.deltaTime;
	}

    // Hostile projectile colliding with the cannon - adjust HP.
    private void OnCollisionEnter(Collision collision) {
        int dmg = Mathf.CeilToInt(collision.relativeVelocity.magnitude * 2.0f);
        this.Health -= dmg;


        if (this.ScoreDisplay) {
            this.ScoreDisplay.text = "" + this.Health;
        }
    }

    private bool CanFire() {
        return this.elapsedTimeSinceLastFire >= this.fireCooldownTime && this._enabled;
    }

    private void ShootProjectile() {
        Vector3 direction = new Vector3(1.0f,0.0f,0.0f);
        direction = direction * this.forceMult;
        direction = this.gameObject.transform.localToWorldMatrix * direction;

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.layer = this.ProjectileLayer;
        ball.transform.position = this.gameObject.transform.position;

        var body = ball.AddComponent<Rigidbody>();
        body.mass = 0.1f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.AddForceAtPosition(direction, body.position);

        var info = new ProjectileInfo(ball, Time.fixedUnscaledTime);
        this.projectiles.Enqueue(info);
    }

    // TODO can make smaller as they approach their max lifetime.
    private void CleanupProjectiles() {
        if (this.projectiles.Count == 0) {
            return;
        }

        float currentTime = Time.fixedUnscaledTime;

        ProjectileInfo info = this.projectiles.Peek();

        while (this.projectiles.Count > 0 && currentTime > info.timeCreated + this.projectileLifetime) {
            this.projectiles.Dequeue();
            Destroy(info.projectile.GetComponent<Rigidbody>());
            Destroy(info.projectile);
            if (this.projectiles.Count != 0) {
                info = this.projectiles.Peek();
            }
        }
    }

    private void SetStatus(bool _enabled) {
        this._enabled = _enabled; 
    }
}
