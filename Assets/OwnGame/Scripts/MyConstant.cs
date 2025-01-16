using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MyConstant
{
    public static long ConvertGuidToInt(Guid guid)
    {
        // Chuyển Guid thành chuỗi
        string guidString = guid.ToString();

        // Sử dụng mã hóa MD5 để tạo một chuỗi băm
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(guidString);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Chuyển chuỗi băm thành một số nguyên
            long result = BitConverter.ToInt32(hashBytes, 0);

            if (result < 0)
                result = -result;

            return result;
        }
    }
    public static long GenerateUUID()
    {
        return ConvertGuidToInt(Guid.NewGuid());
    }
    /// <summary>
    /// Check xem vị trí pos truyền vào có nằm trong góc hình nón của radar không
    /// </summary>
    public static bool CheckIfInVisionCone(Vector2 _currentPosition, Vector2 _forward, float _visionAngle, Vector2 _boidOtherPosition){
        // - Tính Vector từ đối tượng tới vị trí
        Vector2 _directionToPosition = _boidOtherPosition - _currentPosition;
        // - Tính tích vô hướng của vector này tới hướng đối tượng
        float _dotProduct = Vector2.Dot(_forward.normalized, _directionToPosition);
        // - Tính cosin của nửa góc tầm nhìn (visionAngle), chuyển từ độ sang radian. Góc này xác định kích thước của hình nón tầm nhìn
        float _cosHalfVisionAngle = Mathf.Cos(_visionAngle * 0.5f * Mathf.Deg2Rad);
        // - So sánh kết quả với cosin của nửa góc tầm nhìn để xác định vị trí có nằm trong hình nón tầm nhìn hay không
        return _dotProduct >= _cosHalfVisionAngle;
    }
}
