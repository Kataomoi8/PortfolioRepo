#pragma once
#include "pools.h"
/*
* This is an emitter class that I created for my engine development program. I made this emitter to grasp a better understanding of particle systems and physics.
* The emitter class has its own particles, the emitter free class has a shared pool of particles.
* The free emitters have a limited particle pool so they eventually run out of particles to disperse. 
* When the emitters disperse particles they fire them straight up and the particles then fall to the group like a fountain.
* This would simulate a proper physics system in a game engine.
*/

struct particle {
	float3 pos;
	float3 prev_Pos;
	float3 velocity;
	float4 color;
};

class emitter
{
private:
	end::sorted_pool_t<end::particle, 1024> sortPool;
	end::pool_t<end::particle, 1024> freePool;
	end::float3 gravity = { 0.0f, -2.0f, 0.0f };
public:

	float emitTimer = 0.0f;
	float emitTimerFree = 0.0f;
	float emitInterval = 0.005f;

	void update(float deltaTime) {
	
		emitTimer += deltaTime;
		//If the timer was greater than the intervale we would give a particle a random velocity to start it.
		//Then we would add the particle to the sorted pool.
		if (emitTimer > emitInterval)
		{
			int index = sortPool.alloc();
			sortPool[index].velocity = { RANDFLOAT(-0.5f, 0.5f), 4.0f, RANDFLOAT(-0.5f, 0.5f) };
			sortPool[index].pos = { 0.0f, 0.0f, 0.0f };
			sortPool[index].prev_Pos = sortPool[index].pos;
			sortPool[index].color = { 0.0f, 1.0f, 1.0f, 1.0f };
			emitTimer -= emitInterval;
		}
		//I would then apply gravity to each particle in the pool and draw it to the screen. 
		for (int i = 0; i < sortPool.size(); i++)
		{
			sortPool[i].velocity += gravity * deltaTime;
			sortPool[i].prev_Pos = sortPool[i].pos;
			sortPool[i].pos += sortPool[i].velocity * deltaTime;

			end::debug_renderer::add_line(sortPool[i].prev_Pos, sortPool[i].pos, sortPool[i].color);
		}
		//I would only free a particle if it fell below 0 in the y. Freeing would release its spot in the pool.
		for (int i = 0; i < sortPool.size(); i++)
		{
			if (sortPool[i].pos.y < 0.0f)
			{
				sortPool.free(i);
				i--;
			}
		}
	}
};

class emitterFree
{
private:
	end::sorted_pool_t<unsigned int, FREE_SIZE_MODE> indiciesPool;
	end::float3 gravity = { 0.0f, -2.0f, 0.0f };
public:

	float emitTimer = 0.0f;
	float emitInterval = 0.005f;

	end::float3 pos;
	end::float4 color;

	void update(float deltaTime, end::pool_t<end::particle, FREE_SIZE_MODE> &freePool) {

		emitTimer += deltaTime;

		if (emitTimer > emitInterval)
		{
			//Since the free pool has a limited size we would allocate it an index and make sure its not out of space.
			int index = freePool.alloc();
			if (index != -1)
			{
				int freeIndex = indiciesPool.alloc();
				if (freeIndex != -1)
				{
					//Once we know the particle is a particle we can add it to the indiciesPool which is how an emitter claimed a particle.
					indiciesPool[freeIndex] = index;
					//Then we would build the particle at the index in the free pool.
					freePool[index].velocity = { RANDFLOAT(-0.5f, 0.5f), 4.0f, RANDFLOAT(-0.5f, 0.5f) };
					freePool[index].pos = pos;
					freePool[index].prev_Pos = freePool[index].pos;
					freePool[index].color = color;
				}
				else {
					//If all of the active particles are claimed in the indicies pool then we free the particle at the index in the free pool.
					freePool.free(index);
				}
			}
			emitTimer -= emitInterval;
		}

		//Here we apply gravity to each particle in the freepool and then draw it.
		//If the particle falls below 0 in the y we then free it from the free pool and the indicies pool.
		for (int i = 0; i < indiciesPool.size(); i++)
		{
			freePool[indiciesPool[i]].velocity += gravity * deltaTime;
			freePool[indiciesPool[i]].prev_Pos = freePool[indiciesPool[i]].pos;
			freePool[indiciesPool[i]].pos += freePool[indiciesPool[i]].velocity * deltaTime;

			end::debug_renderer::add_line(freePool[indiciesPool[i]].prev_Pos, freePool[indiciesPool[i]].pos, freePool[indiciesPool[i]].color);

			if (freePool[indiciesPool[i]].pos.y < 0.0f)
			{
				freePool.free(indiciesPool[i]);
				indiciesPool.free(i);
				i--;
			}
		}
	}
};

