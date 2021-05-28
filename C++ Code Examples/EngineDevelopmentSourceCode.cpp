/*
* These methods were used to simulate a viewport in an engine that would simulate when the viewport was able to see an aabb in its view.
*/


/*
* This method draws and calculated the user's frustum. The method first finds the centers of the 6 planes.
* Then the method uses the center points to find the corners of the frustum. Once the corners are found the debug_line renders draws out the lines connecting the corners.
* The method also draws the normal of each plane of the frustum and draws a line indicating it.
*/
void calculate_frustum(frustum_t& frustum, XMMATRIX& worldMat)
{
	//float3 newPostion = position + (point * 1);
	float4 red = { 1.0f, 0.0f, 0.0f, 0.0f };
	float4 blue = { 0.0f, 0.0f, 1.0f, 0.0f };
	float3 worldPos;
	worldPos.x = XMVectorGetX(worldMat.r[3]);
	worldPos.y = XMVectorGetY(worldMat.r[3]);
	worldPos.z = XMVectorGetZ(worldMat.r[3]);
	float3 zDir;
	zDir.x = XMVectorGetX(worldMat.r[2]);
	zDir.y = XMVectorGetY(worldMat.r[2]);
	zDir.z = XMVectorGetZ(worldMat.r[2]);
	float3 yDir;
	yDir.x = XMVectorGetX(worldMat.r[1]);
	yDir.y = XMVectorGetY(worldMat.r[1]);
	yDir.z = XMVectorGetZ(worldMat.r[1]);
	float3 xDir;
	xDir.x = XMVectorGetX(worldMat.r[0]);
	xDir.y = XMVectorGetY(worldMat.r[0]);
	xDir.z = XMVectorGetZ(worldMat.r[0]);

	float fov = 45.0f;
	float viewRatio = 0.75f;
	float nearDist = 0.5f;
	float farDist = 5.0f;

	//far
	float3 centFar = worldPos + (zDir * farDist);
	float hFar = 2 * tanf(fov / 2) * farDist;
	float wFar = hFar * viewRatio;
	//near
	float3 centNear = worldPos + (zDir * nearDist);
	float hNear = 2 * tanf(fov / 2) * nearDist;
	float wNear = hNear * viewRatio;

	float3 FTL = centFar + yDir * (hFar * 0.5f) - xDir * (wFar * 0.5f);
	float3 FTR = centFar + yDir * (hFar * 0.5f) + xDir * (wFar * 0.5f);
	float3 FBL = centFar - yDir * (hFar * 0.5f) - xDir * (wFar * 0.5f);
	float3 FBR = centFar - yDir * (hFar * 0.5f) + xDir * (wFar * 0.5f);

	float3 NTL = centNear + yDir * (hNear * 0.5f) - xDir * (wNear * 0.5f);
	float3 NTR = centNear + yDir * (hNear * 0.5f) + xDir * (wNear * 0.5f);
	float3 NBL = centNear - yDir * (hNear * 0.5f) - xDir * (wNear * 0.5f);
	float3 NBR = centNear - yDir * (hNear * 0.5f) + xDir * (wNear * 0.5f);

	debug_renderer::add_line(FTL, FTR, red);
	debug_renderer::add_line(FBL, FBR, red);
	debug_renderer::add_line(FTL, FBL, red);
	debug_renderer::add_line(FTR, FBR, red);

	debug_renderer::add_line(NTL, NTR, red);
	debug_renderer::add_line(NBL, NBR, red);
	debug_renderer::add_line(NTL, NBL, red);
	debug_renderer::add_line(NTR, NBR, red);

	debug_renderer::add_line(NTL, FTL, red);
	debug_renderer::add_line(NTR, FTR, red);
	debug_renderer::add_line(NBL, FBL, red);
	debug_renderer::add_line(NBR, FBR, red);

	//0 is far plane
	//1 is near plane
	//2 is left side plane
	//3 is right side plane
	//4 is bottom plane
	//5 is top plane
	frustum[0] = calculate_plane(FBR, FBL, FTL);
	frustum[1] = calculate_plane(NTL, NBL, NBR);
	frustum[2] = calculate_plane(FBL, NBL, NTL);
	frustum[3] = calculate_plane(NBR, FBR, FTR);
	frustum[4] = calculate_plane(FBR, NBR, NBL);
	frustum[5] = calculate_plane(NTR, FTR, FTL);

	float3 center;
	float3 endPoint = centFar + frustum[0].normal;
	debug_renderer::add_line(centFar, endPoint, red, blue);
	endPoint = centNear + frustum[1].normal;
	debug_renderer::add_line(centNear, endPoint, red, blue);

	center = (NTL + NBL + FBL + FTL) / 4;
	endPoint = center + frustum[2].normal;
	debug_renderer::add_line(center, endPoint, red, blue);
	center = (NTR + NBR + FBR + FTR) / 4;
	endPoint = center + frustum[3].normal;
	debug_renderer::add_line(center, endPoint, red, blue);
	center = (NBL + NBR + FBR + FBL) / 4;
	endPoint = center + frustum[4].normal;
	debug_renderer::add_line(center, endPoint, red, blue);
	center = (NTL + NTR + FTR + FTL) / 4;
	endPoint = center + frustum[5].normal;
	debug_renderer::add_line(center, endPoint, red, blue);
};


/*
* This method is called in the update of the engine program.
* The method takes in an object in the scene and the frustum of the user and checks to see if an object is inside the frustum.
* If an object is inside the frustum it returns true.
*/
bool aabb_to_frustum(const aabb_t& aabb, const frustum_t& frustum)
{

	for (size_t i = 0; i < 6; i++)
	{
		if (classify_aabb_to_plane(aabb, frustum[i]) == -1)
		{
			return false;
		}
	}
	return true;
};

// Calculates which side of a plane the aabb is on.
//
// Returns -1 if the aabb is completely behind the plane.
// Returns 1 if the aabb is completely in front of the plane.
// Otherwise returns 0 (aabb overlaps the plane)
int classify_aabb_to_plane(const aabb_t& aabb, const plane_t& plane)
{
	float3 dotGamer;
	float rad;
	float dotStore;
	rad = (aabb.extents.x * abs(plane.normal.x)) +
		(aabb.extents.y * abs(plane.normal.y)) +
		(aabb.extents.z * abs(plane.normal.z));
	dotStore = dotGamer.dot(aabb.center, plane.normal) - plane.offset;
	if (dotStore > rad)
	{
		return 1;
	}
	if (dotStore < -rad)
	{
		return -1;
	}
	return 0;
};

