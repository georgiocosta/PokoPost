using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public float stoneDamping;
    public float stoneCooldown;
    public float dashTime;

    Vector3 respawnPoint;
    Vector3 lastMoveDirection;
    public Vector3 moveDirection;
    Vector3 snapDirection;
    Vector3 dashDirection;

    private bool isStone, isSnapped, isPaused, isDashing;
    private float stoneCountdown;

    CharacterController controller;
    CameraController cameraController;
    CapsuleCollider playerCollider;
    Rigidbody rb;
    SkinnedMeshRenderer modelRenderer;
    ParticleSystem particles;
    Animator animator;
    AudioSource[] sfx;
    public GameObject statueModel;
    public GameObject boxModel, hatModel;

    void Start()
    {
        respawnPoint = transform.position;
        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        modelRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();
        particles = GetComponent<ParticleSystem>();
        sfx = GetComponents<AudioSource>();
        cameraController = Camera.main.GetComponent<CameraController>();

        lastMoveDirection = Vector3.zero;
        stoneCountdown = 0;
    }

    void Update()
    {
        pauseGame();

        if (!isPaused)
        {
            snapToWall();
            movePlayer();
            switchForm();

            if (stoneCountdown > 0)
                stoneCountdown -= Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.C))
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void pauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            sfx[2].Play();
            isPaused = !isPaused;

            if (isPaused)
                Time.timeScale = 0;
            else if(Time.timeScale == 0)
                Time.timeScale = 1;
        }
    }

    private void movePlayer()
    {
        if (!isStone)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (moveDirection.magnitude > 1f)
                moveDirection = moveDirection.normalized;

            if (isSnapped)
            {
                Vector3 relativeDirection = Vector3.Scale(moveDirection, transform.right);
                //Debug.Log(relativeDirection);

                if (relativeDirection != Vector3.zero)
                {
                    Debug.Log("Direction not zero");
                    animator.SetBool("isShuffling", true);
                    animator.transform.localScale = new Vector3((relativeDirection.normalized.x + relativeDirection.normalized.z) * -0.15f, animator.transform.localScale.y, animator.transform.localScale.z);
                }
                else
                    animator.SetBool("isShuffling", false);

                moveDirection = moveDirection + snapDirection; //* walkSpeed;
                
                //modelRenderer.transform.localScale = new Vector3(modelRenderer.transform.localScale.x * moveDirection.normalized.magnitude, modelRenderer.transform.localScale.y, modelRenderer.transform.localScale.z);
            }
            else if (isDashing)
                moveDirection += dashDirection;
            else
                animator.SetBool("isShuffling", false);

            controller.Move(moveDirection * walkSpeed * Time.deltaTime);

            if (moveDirection == Vector3.zero || isSnapped)
                animator.SetBool("isRunning", false);

            if (moveDirection != Vector3.zero && !isSnapped)
            {
                //transform.rotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.25f);
                animator.SetBool("isRunning", true);
            }
            else if (isSnapped)
            {
                transform.rotation = Quaternion.LookRotation(-snapDirection);
            }

            lastMoveDirection = moveDirection;
        }

        else if(isStone)
        {
            if (lastMoveDirection.magnitude > 1f)
                lastMoveDirection = lastMoveDirection.normalized;

            if (isSnapped)
                moveDirection = lastMoveDirection + snapDirection; //* walkSpeed;

            controller.Move(lastMoveDirection * walkSpeed * Time.deltaTime);

            if(lastMoveDirection != Vector3.zero)
            {
                lastMoveDirection -= lastMoveDirection * stoneDamping;
            }
        }
    }

    private void snapToWall()
    {
        if (isSnapped)
        {
            if (!Physics.Raycast(transform.position, transform.forward * -1, 2f))
            {
                UnSnap();
                sfx[3].Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit hit;
            if (!isSnapped && Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
            {
                if (hit.transform.CompareTag("Wall"))
                {
                    sfx[3].Play();

                    snapDirection = -hit.normal;

                    controller.enabled = false;
                    transform.position = hit.point + hit.normal;
                    controller.enabled = true;

                    isSnapped = true;
                }
            }
            else if (isSnapped)
            {
                sfx[3].Play();
                UnSnap();
            }

        }
    }

    private void UnSnap()
    {
        isSnapped = false;
        //controller.Move(-snapDirection * 5 * Time.deltaTime);
        isDashing = true;
        dashDirection = -snapDirection;
        Invoke("StopDash", dashTime);
    }

    private void switchForm()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isStone && stoneCountdown <= 0)
        {
            animateTransition();
            isStone = true;
            Invoke("turnToStone", 0.1f);
            stoneCountdown = stoneCooldown;
        }
        if(Input.GetKeyUp(KeyCode.Z) && isStone)
        {
            CancelInvoke();
            isDashing = false;
            animateTransition();
            Invoke("revert", 0.1f);
        }
    }

    private void animateTransition()
    {
        modelRenderer.enabled = false;
        statueModel.SetActive(false);
        boxModel.SetActive(false);
        hatModel.SetActive(false);

        particles.Play();
        sfx[1].Play();

        gameObject.layer = 9;
    }

    private void turnToStone()
    {
        switchForm();
        statueModel.SetActive(true);
        //gameObject.layer = 9;
        Invoke("animateTransition", 0.9f);
        Invoke("revert", 1f);
        Invoke("StartShaking", 0.75f);
    } 

    private void revert()
    {
        statueModel.transform.localPosition = new Vector3(0, -0.95f, 0);
        isStone = false;
        switchForm();
        modelRenderer.enabled = true;
        boxModel.SetActive(true);
        hatModel.SetActive(true);
        gameObject.layer = 8;
    }

    public void Die()
    {
        particles.Play();
        sfx[0].Play();
        playerCollider.enabled = false;
        modelRenderer.enabled = false;
        boxModel.SetActive(false);
        hatModel.SetActive(false);
        controller.enabled = false;
        stoneCountdown = stoneCooldown;
        cameraController.SendMessage("StartShake");
        Invoke("Respawn", .2f);
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        playerCollider.enabled = true;
        modelRenderer.enabled = true;
        controller.enabled = true;
        boxModel.SetActive(true);
        hatModel.SetActive(true);
    }

    public void StartShaking()
    {
        StartCoroutine(ShakeStatue());
    }

    public IEnumerator ShakeStatue()
    {
        float timePassed = 0f;

        while (timePassed < 0.25f)
        {
            statueModel.transform.localPosition = new Vector3(0, -0.95f, 0);

            float x = Random.Range(-0.1f, 0.1f);
            float z = Random.Range(-0.1f, 0.1f);

            statueModel.transform.position = new Vector3(statueModel.transform.position.x + x, statueModel.transform.position.y, statueModel.transform.position.z + z);
            timePassed += Time.deltaTime;
            yield return 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Goal"))
        {
            GameObject.Find("UI").GetComponentInChildren<Timer>().SendMessage("StopCount");
            sfx[4].Play();
            //SceneManager.LoadSceneAsync("LevelSelect");
        }
        else if(other.CompareTag("Enemy") && !isStone)
        {
            Die();
        }
    }

    private void StopDash()
    {
        isDashing = false;
        dashDirection = Vector3.zero;
    }
}
