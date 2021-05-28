/*
* Section for different variables/data structures that held information
*/
{
	struct anim_clipImport {
		double duration;
		std::vector<keyframeImport> frames;
	};

	end::simple_mesh mash;
	std::vector<uint32_t> indicies2;
	std::vector<end::simple_vert> verts2;
	std::vector<end::joint> skeleJoints;
	std::vector<end::jointXM> skeleJointsXM;
	anim_clipImport animationImport;
}
/*
* This read file method was used in my skinning models program.
* The file would read in the binary data of the models skeleton and its animation frame data.
* Once these were read in they would be stored in different data structures.
*
*/
void read_file(const char* file_path) {

	std::fstream file{ file_path, std::ios_base::in | std::ios_base::binary };

	if (!file.is_open())
	{
		return;
	}

	file.read((char*)&mash.index_count, sizeof(uint32_t));
	indicies2.resize(mash.index_count);

	file.read((char*)indicies2.data(), sizeof(uint32_t) * mash.index_count);

	file.read((char*)&mash.vert_count, sizeof(uint32_t));
	verts2.resize(mash.vert_count);

	file.read((char*)verts2.data(), sizeof(end::simple_vert) * mash.vert_count);

	uint32_t skeleJointCount = 0;
	file.read((char*)&skeleJointCount, sizeof(uint32_t));
	skeleJoints.resize(skeleJointCount);
	file.read((char*)skeleJoints.data(), sizeof(end::joint) * skeleJointCount);

	//file.read((char*)&animationImport, sizeof(anim_clipImport));
	uint32_t Count = 0;
	file.read((char*)&Count, sizeof(uint32_t));
	animationImport.frames.resize(Count);
	for (size_t i = 0; i < Count; i++)
	{
		uint32_t jointSize = 0;
		file.read((char*)&jointSize, sizeof(uint32_t));
		animationImport.frames[i].joints.resize(jointSize);
		file.read((char*)animationImport.frames[i].joints.data(), sizeof(end::floaty4x4) * jointSize);
		file.read((char*)&animationImport.frames[i].time, sizeof(double));
	}
	file.read((char*)&animationImport.duration, sizeof(double));
}

/*
* This function would convert the array of joint data that was read in into a matrix.
* When in matrix form it would be more compatible with DirectX Math Library
*/
void convertArrToMatrix()
{
	int slot4 = 0;
	int slot16 = 0;
	int row = 0;
	for (size_t i = 0; i < skeleJoints.size(); i++)
	{
		end::jointXM tempJoint;
		while (slot16 != 16)
		{
			tempJoint.global_xform[row][slot4] = skeleJoints[i].global_xform[slot16];
			slot4++;
			slot16++;
			if (slot4 == 4)
			{
				slot4 = 0;
				row++;
				if (row == 4)
				{
					row = 0;
				}
			}
		}
		tempJoint.parent_index = skeleJoints[i].parent_index;
		skeleJointsXM.push_back(tempJoint);
		if (slot16 == 16)
		{
			slot16 = 0;
		}
	}
}
/*
* This function was used for the camera matrix that the user would look through in the program.
* This function made sure when the camera would rotate the matrix wouldn't invert itself.
* If the matrix inverted itself it would cause the user view to get crazy and the view would flip over on itself.
*/
void renderer_t::StabilizeMatrix(XMMATRIX& mat) {
	XMVECTOR globalY = { 0,1,0 };
	XMVECTOR vNewX = XMVector3Cross(globalY, mat.r[2]);
	XMVECTOR vNewY = XMVector3Cross(mat.r[2], vNewX);

	mat.r[0] = XMVector3Normalize(vNewX);
	mat.r[1] = XMVector3Normalize(vNewY);
	mat.r[2] = XMVector3Normalize(mat.r[2]);
}
/*
* This function was used to have the camera view matrix always point its forward toward the mouse cursor.
* This would simulate a first person point of view.
*/
float4x4_a renderer_t::mouseLook(float4x4_a mat, float dX, float dY) {
	XMMATRIX rotateY;
	XMMATRIX rotateX;
	if (dX > 0 || dY > 0 || dX < 0 || dY < 0)
	{
		float rotY = dX * (mouseSens * timer.TotalTime());
		float rotX = dY * (mouseSens * timer.TotalTime());
		rotateY = XMMatrixRotationY(rotY);
		rotateX = XMMatrixRotationX(rotX);
	}
	else {
		rotateY = XMMatrixRotationY(0);
		rotateX = XMMatrixRotationX(0);
	}
	XMMATRIX temp = convertToXMMAT(mat);
	temp = XMMatrixMultiply(rotateY, temp);
	temp = XMMatrixMultiply(rotateX, temp);
	StabilizeMatrix(temp);

	return convertTofloat4x4(temp);
}

/*
* Controls for the user camera.
* These are simple movements that would move the user point of view based on time.
*/
{
	/*
	* Controls for inside the application
	* Arrow UP: Moves Animation Forward
	* Arrow Down: Moves Animation Backward
	* W key: Moves Camera Forward
	* S Key: Moves Camera Backward
	* A Key: Moves Camera Left
	* D Key: Moves Camera Right
	* Right Mouse Button: Enables you to move the camera around with your mouse
	*/
	XTime timer;
	XMMATRIX translate;
	XMMATRIX tempStore;
	float speed = 20;
	buttonDelay += timer.Delta();
	if (boolsKeys[VK_UP] == 1 && buttonDelay >= 0.05f)
	{
		frame += 1;
		buttonDelay -= buttonDelay;
		frameDelay = 0;
	}
	if (boolsKeys[VK_DOWN] == 1 && buttonDelay >= 0.05f)
	{
		frame -= 1;
		buttonDelay -= buttonDelay;
		frameDelay = 0;
	}
	if (boolsKeys['W'] == 1)
	{
		translate = XMMatrixTranslation(0, 0, timer.Delta() * speed);
		tempStore = *(XMMATRIX*)&default_view.view_mat;
		tempStore = XMMatrixMultiply(translate, tempStore);
		default_view.view_mat = *(float4x4_a*)&tempStore;

	}
	if (boolsKeys['S'] == 1)
	{
		translate = XMMatrixTranslation(0, 0, -timer.Delta() * speed);
		tempStore = *(XMMATRIX*)&default_view.view_mat;
		tempStore = XMMatrixMultiply(translate, tempStore);
		default_view.view_mat = *(float4x4_a*)&tempStore;
	}
	if (boolsKeys['A'] == 1)
	{
		translate = XMMatrixTranslation(-timer.Delta() * speed, 0, 0);
		tempStore = *(XMMATRIX*)&default_view.view_mat;
		tempStore = XMMatrixMultiply(translate, tempStore);
		default_view.view_mat = *(float4x4_a*)&tempStore;
	}
	if (boolsKeys['D'] == 1)
	{
		translate = XMMatrixTranslation(timer.Delta() * speed, 0, 0);
		tempStore = *(XMMATRIX*)&default_view.view_mat;
		tempStore = XMMatrixMultiply(translate, tempStore);
		default_view.view_mat = *(float4x4_a*)&tempStore;
	}
	if (boolsKeys[VK_RBUTTON] == 1)
	{
		float dX = xMouse - xPrevMouse;
		float dY = yMouse - yPrevMouse;
		default_view.view_mat = mouseLook(default_view.view_mat, dX, dY);
	}
	else {
		default_view.view_mat = mouseLook(default_view.view_mat, 0, 0);
	}
	xPrevMouse = xMouse;
	yPrevMouse = yMouse;
}
