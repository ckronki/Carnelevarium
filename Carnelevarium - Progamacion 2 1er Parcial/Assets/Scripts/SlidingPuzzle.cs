using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // nuevo sistema

public class SlidingPuzzle : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

    [SerializeField] private GameObject FreeFall;

    private List<Transform> pieces;
    private int emptyLocation;
    private int size;
    private bool shuffling = false;

    // Crear el tablero con piezas size x size
    private void CreateGamePieces(float gapThickness)
    {
        float width = 1f / size;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);

                piece.localPosition = new Vector3(
                    -1 + (2 * width * col) + width,
                    +1 - (2 * width * row) - width,
                    0
                );

                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";

                // Última pieza: espacio vacío
                if (row == size - 1 && col == size - 1)
                {
                    emptyLocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

                    mesh.uv = uv;
                }
            }
        }
    }

    void Start()
    {
        pieces = new List<Transform>();
        size = 3;
        CreateGamePieces(0.01f);

        Shuffle();
    }

    void Update()
    {
        if (!shuffling && CheckCompletion())
        {
            shuffling = true;
            ShowVictory();
        }

        // Nuevo Input System + Raycast 3D
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {

                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        if (SwapIfValid(i, -size, size)) break;
                        if (SwapIfValid(i, +size, size)) break;
                        if (SwapIfValid(i, -1, 0)) break;
                        if (SwapIfValid(i, +1, size - 1)) break;
                    }
                }
            }
        }
    }

    private void ShowVictory()
    {
        Debug.Log("¡Puzzle resuelto!");

        // Bloquear todas las piezas
        foreach (Transform piece in pieces)
        {
            Collider col = piece.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        // Acción en el mundo 3D
        if (FreeFall != null)
        {
            Animator anim = FreeFall.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Fall"); // dispara animación de apertura
        }
    }

    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        int targetIndex = i + offset;

        if (targetIndex < 0 || targetIndex >= pieces.Count) return false;
        if ((i % size) == colCheck) return false;

        if (targetIndex == emptyLocation)
        {
            (pieces[i], pieces[targetIndex]) = (pieces[targetIndex], pieces[i]);
            (pieces[i].localPosition, pieces[targetIndex].localPosition) =
                (pieces[targetIndex].localPosition, pieces[i].localPosition);

            emptyLocation = i;

            // 🔹 Condición de victoria
            if (CheckCompletion())
            {
                ShowVictory();
            }

            return true;
        }
        return false;
    }

    private bool CheckCompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
                return false;
        }
        return true;
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        int count = 0;
        int last = 0;

        while (count < (size * size * size))
        {
            int rnd = Random.Range(0, size * size);
            if (rnd == last) continue;
            last = emptyLocation;

            if (SwapIfValid(rnd, -size, size)) count++;
            else if (SwapIfValid(rnd, +size, size)) count++;
            else if (SwapIfValid(rnd, -1, 0)) count++;
            else if (SwapIfValid(rnd, +1, size - 1)) count++;
        }
    }
}
