using System.Collections.Generic;
using UnityEngine;
using nuitrack;
using Vector3 = UnityEngine.Vector3;

public class SimpleSkeletonAvatar : MonoBehaviour
{
    public bool autoProcessing = true;
    [SerializeField] GameObject jointPrefab = null, connectionPrefab = null; //, gestureBarContainerPrefab = null, gestureBar = null;


    [HideInInspector]
    public int playerIndex;
    private GameController gameController;

    JointType[] jointsInfo = new JointType[]
    {
        JointType.Head,
        JointType.Neck,
        JointType.LeftCollar,
        JointType.Torso,
        JointType.Waist,
        JointType.LeftShoulder,
        JointType.RightShoulder,
        JointType.LeftElbow,
        JointType.RightElbow,
        JointType.LeftWrist,
        JointType.RightWrist,
        JointType.LeftHand,
        JointType.RightHand,
        JointType.LeftHip,
        JointType.RightHip,
        JointType.LeftKnee,
        JointType.RightKnee,
        JointType.LeftAnkle,
        JointType.RightAnkle
    };

    JointType[,] connectionsInfo = new JointType[,]
    { //Right and left collars are currently located at the same point, that's why we use only 1 collar,
        //it's easy to add rightCollar, if it ever changes
        {JointType.Neck,           JointType.Head},
        {JointType.LeftCollar,     JointType.Neck},
        {JointType.LeftCollar,     JointType.LeftShoulder},
        {JointType.LeftCollar,     JointType.RightShoulder},
        {JointType.LeftCollar,     JointType.Torso},
        {JointType.Waist,          JointType.Torso},
        {JointType.Waist,          JointType.LeftHip},
        {JointType.Waist,          JointType.RightHip},
        {JointType.LeftShoulder,   JointType.LeftElbow},
        {JointType.LeftElbow,      JointType.LeftWrist},
        {JointType.LeftWrist,      JointType.LeftHand},
        {JointType.RightShoulder,  JointType.RightElbow},
        {JointType.RightElbow,     JointType.RightWrist},
        {JointType.RightWrist,     JointType.RightHand},
        {JointType.LeftHip,        JointType.LeftKnee},
        {JointType.LeftKnee,       JointType.LeftAnkle},
        {JointType.RightHip,       JointType.RightKnee},
        {JointType.RightKnee,      JointType.RightAnkle}
    };

    GameObject[] connections;
    Dictionary<JointType, GameObject> joints;

    GameObject gestureBarContainer;

    void Start() {
        // base.Start();
        this.gameController = GameObject.Find("Game Controller").GetComponent<GameController>();

        CreateSkeletonParts();

    }

    void CreateSkeletonParts() {
        joints = new Dictionary<JointType, GameObject>();

        for (int i = 0; i < jointsInfo.Length; i++) {
            if (jointPrefab != null) {
                GameObject joint = Instantiate(jointPrefab, transform, true);
                joint.SetActive(false);
                joints.Add(jointsInfo[i], joint);
            }
        }

        connections = new GameObject[connectionsInfo.GetLength(0)];

        for (int i = 0; i < connections.Length; i++) {
            if (connectionPrefab != null) {
                GameObject connection = Instantiate(connectionPrefab, transform, true);
                connection.SetActive(false);
                connections[i] = connection;
            }
        }

        // Gesture Bar Setup

        //this.gestureBarContainer = Instantiate(gestureBarContainerPrefab, transform, true);
        //var barController = this.gestureBarContainer.GetComponent<GestureBarsController>();
        //barController.player = this;
        //barController.gestureBar = this.gestureBar;
    }

    void Update() {
        if (autoProcessing) {
            ProcessSkeleton(CurrentUserTracker.CurrentSkeleton);
        }
        ProcessSkeletonGesture();
        //base.Update();
    }


    public void ProcessSkeleton(Skeleton skeleton) {
        if (skeleton == null)
            return;

        for (int i = 0; i < jointsInfo.Length; i++) {
            nuitrack.Joint j = skeleton.GetJoint(jointsInfo[i]);
            if (j.Confidence > 0.5f) {
                joints[jointsInfo[i]].SetActive(true);
                joints[jointsInfo[i]].transform.position = new Vector2(j.Proj.X * Screen.width, Screen.height - j.Proj.Y * 344f);
            } else {
                joints[jointsInfo[i]].SetActive(false);
            }
        }

        for (int i = 0; i < connectionsInfo.GetLength(0); i++) {
            GameObject startJoint = joints[connectionsInfo[i, 0]];
            GameObject endJoint = joints[connectionsInfo[i, 1]];

            if (startJoint.activeSelf && endJoint.activeSelf) {
                connections[i].SetActive(true);

                connections[i].transform.position = startJoint.transform.position;
                connections[i].transform.right = endJoint.transform.position - startJoint.transform.position;
                float distance = Vector3.Distance(endJoint.transform.position, startJoint.transform.position);
                connections[i].transform.localScale = new Vector3(distance, 1f, 1f);
            } else {
                connections[i].SetActive(false);
            }
        }

        this.waistRollingAvgY = (JointPosition(JointType.Waist).y + this.waistRollingAvgY) / 2;
        this.gestureBarContainer.transform.position = JointPosition(JointType.Head) + new Vector3(50, 0, 0);

    }

    public const int GESTURE_THRESHOLD_DISTANCE = 15;
    public const int GESTURE_LEAN_THRESHOLD_DISTANCE = 40;
    public const int GESTURE_JUMPING_THRESHOLD_DISTANCE = 10;
    private float waistRollingAvgY = 0;

    // Process Joint Gesture

    private void ProcessSkeletonGesture() {

        if (IsLeftHandUp() && IsRightHandUp()) {
            // BothHand
            GestureAction(PlayerGesture.BothHand);

        } else if (IsLeftHandUp()) {
            // LeftHand
            GestureAction(PlayerGesture.LeftHand);

        } else if (IsRightHandUp()) {
            // RightHand
            GestureAction(PlayerGesture.RightHand);

        } else if (IsLeftLean()) {
            // Jump
            GestureAction(PlayerGesture.LeftLean);

        } else if (IsRightLean()) {
            // LeftLean
            GestureAction(PlayerGesture.RightLean);

        } else if (JointPosition(JointType.Waist).y > this.waistRollingAvgY &&
            Mathf.Abs(JointPosition(JointType.Waist).y - this.waistRollingAvgY) > GESTURE_JUMPING_THRESHOLD_DISTANCE) {
            // RightLean
            GestureAction(PlayerGesture.Jump);
        }

    }

    void GestureAction(PlayerGesture gesture) {
        this.gameController.players[this.playerIndex].GestureAction(gesture);
    }


    // Joint Hand

    private bool IsLeftHandUp() {
        return JointPosition(JointType.LeftHand).y > JointPosition(JointType.Neck).y && JointDeltaY(JointType.Neck, JointType.LeftHand) > GESTURE_THRESHOLD_DISTANCE;
    }

    private bool IsRightHandUp() {
        return JointPosition(JointType.RightHand).y > JointPosition(JointType.Neck).y && JointDeltaY(JointType.Neck, JointType.RightHand) > GESTURE_THRESHOLD_DISTANCE;
    }

    private bool IsLeftLean() {
        return JointPosition(JointType.Head).x > JointPosition(JointType.Waist).x && JointDeltaX(JointType.Head, JointType.Waist) > GESTURE_LEAN_THRESHOLD_DISTANCE;
    }

    private bool IsRightLean() {
        return JointPosition(JointType.Head).x < JointPosition(JointType.Waist).x && JointDeltaX(JointType.Head, JointType.Waist) > GESTURE_LEAN_THRESHOLD_DISTANCE;
    }


    // Joint Property Getter

    private Vector3 JointPosition(JointType type) {
        return this.joints[type].transform.position;
    }

    private float JointDeltaX(JointType typeA, JointType typeB) {
        return Mathf.Abs(JointPosition(typeA).x - JointPosition(typeB).x);
    }

    private float JointDeltaY(JointType typeA, JointType typeB) {
        return Mathf.Abs(JointPosition(typeA).y - JointPosition(typeB).y);
    }

    private float JointDistance(JointType typeA, JointType typeB) {
        return Vector3.Distance(JointPosition(typeA), JointPosition(typeB));
    }
}
