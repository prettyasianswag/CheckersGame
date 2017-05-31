using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GGL;

public class PiecePicker : MonoBehaviour
{
    public float pieceHeight = 5f;
    public float rayDistance = 1000f;
    public LayerMask selectionIgnoreLayer;

    private Piece selectedPiece;
    private CheckerBoard board;
    private Vector3 hitPoint;

    // Use this for initialization
    void Start()
    {
        // Find the checkerboard in the scene
        board = FindObjectOfType<CheckerBoard>();
        // Check errors
        if (board == null)
        {
            Debug.LogError("There is no CheckerBoard in the scene!");
        }
    }

    // Check if we are selecting a piece
    void CheckSelection()
    {
        // If there is already a selected piece
        if (selectedPiece != null)
            return; // Exit the function

        // Creating a ray from camera mouse position to world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GizmosGL.color = Color.red;
        GizmosGL.AddLine(ray.origin, ray.origin + ray.direction * rayDistance, 0.1f, 0.1f);
        // Check if the player hits the mouse button
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            // Cast a ray to detect piece
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                // Set the selected piece to be the hit object
                selectedPiece = hit.collider.GetComponent<Piece>();
                // If the user did not hit a piece
                if(selectedPiece == null)
                {
                    Debug.Log("Cannot pick up object: " + hit.collider.name);
                }
            }
        }
    }

    // Move the selected piece if one is selected
    void MoveSelection()
    {
        // Check if a piece has been selected
        if(selectedPiece != null)
        {
            // Create a new ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            GizmosGL.color = Color.yellow;
            GizmosGL.AddLine(ray.origin, ray.origin + ray.direction * rayDistance, 0.1f, 0.1f);
            RaycastHit hit;
            // Raycast to only hit objects that aren't pieces
            if(Physics.Raycast(ray, out hit, rayDistance, ~selectionIgnoreLayer))
            {
                hitPoint = hit.point;
                // Obtain the hit point
                GizmosGL.color = Color.blue;
                GizmosGL.AddSphere(hit.point, 0.5f);
                // Move the piece to position
                Vector3 piecePos = hit.point + Vector3.up * pieceHeight;
                selectedPiece.transform.position = piecePos;
            }

            // Check if moust button is released
            if (Input.GetMouseButtonUp (0))
            {
                // Drop the piece to hitPoint
                Piece piece = selectedPiece.GetComponent<Piece>();
                board.DropPiece(piece, hitPoint);

                // Deselect the piece
                selectedPiece = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSelection();
        MoveSelection();
    }
}
