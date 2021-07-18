using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Text;

internal static class customTypes
{
    internal static void register()
    {
        PhotonPeer.RegisterType(typeof(noticePoint), (byte)'A', serializeNoticePoint, deserializeNoticePoint);
    }

    private static short serializeNoticePoint(StreamBuffer outStream, object customobject)
    {
        noticePoint nPInfo = (noticePoint)customobject;
        int sizeOfObjShapBytes = Encoding.UTF8.GetByteCount(nPInfo.objShap);
        int sizeOfMaterialNam = Encoding.UTF8.GetByteCount(nPInfo.materialNam);
        int sizeOfnP = 4 * 3 * 3 + 4 + sizeOfObjShapBytes + sizeOfMaterialNam + 2;
        byte[] totalBytes = new byte[sizeOfnP];
        int off = 0;
        Protocol.Serialize(nPInfo.pos.x, totalBytes, ref off);
        Protocol.Serialize(nPInfo.pos.y, totalBytes, ref off);
        Protocol.Serialize(nPInfo.pos.z, totalBytes, ref off);
        Protocol.Serialize(nPInfo.rot.x, totalBytes, ref off);
        Protocol.Serialize(nPInfo.rot.y, totalBytes, ref off);
        Protocol.Serialize(nPInfo.rot.z, totalBytes, ref off);
        Protocol.Serialize(nPInfo.sca.x, totalBytes, ref off);
        Protocol.Serialize(nPInfo.sca.y, totalBytes, ref off);
        Protocol.Serialize(nPInfo.sca.z, totalBytes, ref off);
        Protocol.Serialize(nPInfo.stag, totalBytes, ref off);
        System.Buffer.BlockCopy(Encoding.UTF8.GetBytes(nPInfo.objShap), 0, totalBytes, off, sizeOfObjShapBytes);
        off += sizeOfObjShapBytes;
        //identify the end of string
        totalBytes[off] = 0;
        off += 1;
        System.Buffer.BlockCopy(Encoding.UTF8.GetBytes(nPInfo.materialNam), 0, totalBytes, off, sizeOfMaterialNam);
        off += sizeOfMaterialNam;
        //identify the end of string
        totalBytes[off] = 0;
        off += 1;
        outStream.Write(totalBytes, 0, sizeOfnP);

        Debug.Log("len:" + sizeOfnP);
        return (short)sizeOfnP;
    }

    private static object deserializeNoticePoint(StreamBuffer inStream, short length)
    {
        noticePoint nPInfoOut = new noticePoint();
        byte[] totalBytes = new byte[length];
        inStream.Read(totalBytes, 0, length);
        int off = 0;
        Protocol.Deserialize(out nPInfoOut.pos.x, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.pos.y, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.pos.z, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.rot.x, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.rot.y, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.rot.z, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.sca.x, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.sca.y, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.sca.z, totalBytes, ref off);
        Protocol.Deserialize(out nPInfoOut.stag, totalBytes, ref off);
        //find the end of string first
        int endOfObjShap = off;
        while (totalBytes[endOfObjShap] != 0)
        {
            endOfObjShap++;
        }
        //count size of string (bytes)
        int sizeOfObjShap = endOfObjShap - off;
        byte[] tmp1 = new byte[sizeOfObjShap];
        System.Buffer.BlockCopy(totalBytes, off, tmp1, 0, sizeOfObjShap);
        nPInfoOut.objShap = Encoding.UTF8.GetString(tmp1);
        //offset move foward (including identity of the end of string)
        off += sizeOfObjShap;
        off += 1;
        //find the end of string first
        int endOfMaterialNam = off;
        while (totalBytes[endOfMaterialNam] != 0)
        {
            endOfMaterialNam++;
        }
        //count size of string (bytes)
        int sizeOfMaterialNam = endOfMaterialNam - off;
        byte[] tmp2 = new byte[sizeOfMaterialNam];
        System.Buffer.BlockCopy(totalBytes, off, tmp2, 0, sizeOfMaterialNam);
        nPInfoOut.materialNam = Encoding.UTF8.GetString(tmp2);
        off += sizeOfMaterialNam;
        off += 1;
        Debug.Log(nPInfoOut.materialNam);
        return nPInfoOut;
    }
}
