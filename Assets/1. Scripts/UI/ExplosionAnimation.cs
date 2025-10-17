using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public interface IGetCanvase
{
    void GetCanvase(Canvas canvas);
}

public interface ISetTarget
{
    void SetTarget(Transform target);
}


public interface IStartExplosionAnim
{
    void StartExplosionAnim(UnityAction done);
}

public class ExplosionAnimation : MonoBehaviour, IStartExplosionAnim, ISetTarget, IGetCanvase
{

    // 24fps(기본적인 애니메이션)        1/24 ≈ 0.0417
    // 12fps(도트 게임에서 자주 씀)      1/12 ≈ 0.0833
    // 10fps	                        0.1
    // 6fps	                            0.166

    // 빠른 폭발 이펙트   0.05 ~ 0.08 (20~12 fps)
    // 느릿한 불꽃 효과나 잔해 퍼짐	0.1 ~ 0.15 (10~6 fps)
    // m_frameRate = 0.03 이면 0.72초

    Canvas m_canvas;

    public float m_frameRate = 0.08f;

    Transform m_target;

    public Sprite[] m_sprites;
    public SpriteRenderer m_sr;


    int m_frame;

    public void GetCanvase(Canvas canvas)
    {
        if (m_canvas == null)
        {
            m_canvas = canvas;
        }
    }

    public void StartExplosionAnim(UnityAction done)
    {
        StopAllCoroutines();
        StartCoroutine(ExplosionAnim(done));
    }

    IEnumerator ExplosionAnim(UnityAction done)
    {
        float time = 0;
        while (m_frame < m_sprites.Length)
        {
            time += Time.deltaTime;
            if (time >= m_frameRate)
            {
                time = 0;
                m_sr.sprite = m_sprites[m_frame];
                m_frame++;
            }
            yield return null;
        }

        done?.Invoke();
        ManagerManager.Instance.objectPoolManager.Return(gameObject);
        m_target = null;
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
        if (m_target != null)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(m_target.position);

            // 2. 화면 좌표를 Canvas 좌표로 변환
            Vector2 canvasPos;
            RectTransform canvasRectTransform = m_canvas.GetComponent<RectTransform>();
            RectTransform rect = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, Camera.main, out canvasPos);

            // 3. Explosion 위치 설정
            rect.anchoredPosition = canvasPos;
        }
        
    }

}
