


/*
* Area where I stored matrix values for rendering objects that need to be sent to the constant buffer.
*/
struct WVP
{
    XMFLOAT4X4 wMatrix;
    XMFLOAT4X4 vMatrix;
    XMFLOAT4X4 pMatrix;
    XMFLOAT4   camPosition;
    XMFLOAT4 isInstance;

}MyMatricies;

/*
* Variables needed for the Providence rendering
*/
Microsoft::WRL::ComPtr <ID3D11Buffer> vertexBufferMeshProvidence;
Microsoft::WRL::ComPtr <ID3D11Buffer> indexBufferMeshProvidence;
Microsoft::WRL::ComPtr <ID3D11Texture2D> textureProvidence;
Microsoft::WRL::ComPtr <ID3D11ShaderResourceView> srvProvidence;
ID3D11DeviceContext* myCon;

void update()
{
    /*
    * This is a rendering on the Providence space ship object.
    * This block of code shows the set up for the shaders needed and the buffers needed to render it.
    */
    XMMATRIX temp = XMMatrixIdentity();
    XMMATRIX tempSpace = XMMatrixIdendity();

    myCon->PSSetShader(meshPixelShader, 0, 0);
    myCon->IASetVertexBuffers(0, 1, vertexBufferMeshProvidence.GetAddressOf(), meshstrides, meshoffset);
    myCon->IASetIndexBuffer(indexBufferMeshProvidence.Get(), DXGI_FORMAT_R32_UINT, 0);
    temp = XMMatrixIdentity();
    temp = XMMatrixScaling(0.0025f, 0.0025f, 0.0025f);
    tempSpace = XMMatrixTranslation(-3800, 0, -500);
    tempSpace = XMMatrixMultiply(temp, tempSpace);
    temp = XMMatrixRotationY(-90);
    tempSpace = XMMatrixMultiply(temp, tempSpace);
    XMStoreFloat4x4(&MyMatricies.wMatrix, tempSpace);
    myCon->Map(constantBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &gpuBuffer);
    *((WVP*)(gpuBuffer.pData)) = MyMatricies;
    myCon->Unmap(constantBuffer, 0);
    myCon->PSSetShaderResources(0, 1, srvProvidence.GetAddressOf());
    myCon->DrawIndexed(providence_indexcount, 0, 0);
}

/*
* This section is loading the vertex data onto the graphics card so it has its own spot.
* This also creats a buffer for the object at the spot the data is loaded.
*/
void loadData() {
    // load our complex mesh onto the card
    bDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bDesc.ByteWidth = sizeof(providence_data);
    bDesc.CPUAccessFlags = 0;
    bDesc.MiscFlags = 0;
    bDesc.StructureByteStride = 0;
    bDesc.Usage = D3D11_USAGE_IMMUTABLE;

    subData.pSysMem = providence_data;
    hr = myDev->CreateBuffer(&bDesc, &subData, vertexBufferMeshProvidence.GetAddressOf()); // vertex buffer for the mesh
    // index buffer for the mesh
    bDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    bDesc.ByteWidth = sizeof(providence_indicies);

    subData.pSysMem = providence_indicies;
    hr = myDev->CreateBuffer(&bDesc, &subData, &indexBufferMeshProvidence);

    //load texture
    hr = CreateDDSTextureFromFile(myDev, L"Assets/Theme Objects/providence class cruiser/Sev_providence.dds", (ID3D11Resource**)textureProvidence.GetAddressOf(), srvProvidence.GetAddressOf());
}