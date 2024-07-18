using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScreenWrapping : MonoBehaviour
{
    Rigidbody2D rigidbody_2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ScreenWrap();
    }

    private void ScreenWrap()
    {
        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        //Move these calls to start function / have these be called when the screen is resized
        Vector2 rightAndBottomScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 leftAndTopScreen = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f));
        /*float rightSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        float leftSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x;
        float topOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        float bottomOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;*/

        if (screenPosition.x <= 0 && rigidbody_2D.velocity.x < 0)
        {
            transform.position = new Vector2(rightAndBottomScreen.x, transform.position.y);
        }
        else if (screenPosition.x >= Screen.width && rigidbody_2D.velocity.x > 0)
        {
            transform.position = new Vector2(leftAndTopScreen.x, transform.position.y);
        }
        else if (screenPosition.y <= 0 && rigidbody_2D.velocity.y < 0)
        {
            transform.position = new Vector2(transform.position.x, rightAndBottomScreen.y);
        }
        else if (screenPosition.y >= Screen.height && rigidbody_2D.velocity.y > 0)
        {
            transform.position = new Vector2(transform.position.x, leftAndTopScreen.y);
        }
    }
}
