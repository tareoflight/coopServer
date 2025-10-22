use spacetimedb::{reducer, table, Local, ReducerContext, Table};

#[table(name = chunk_data, public, index(name = position, btree(columns = [x, y, z])))]
pub struct ChunkData {
    #[primary_key]
    #[auto_inc]
    id: u64,
    x: i32,
    y: i32,
    z: i32,
}

#[table(name = node_data, public, index(name = position, btree(columns = [x, y, z])))]
pub struct NodeData {
    #[primary_key]
    #[auto_inc]
    id: u64,
    chunk_id: u64, // FK ChunkData.id
    x: i32,
    y: i32,
    z: i32,
    contents: Vec<i32>,
}

#[table(name = node_neighbor, public)]
pub struct NodeNeighbor {
    node_id: u64,
    neighbor_id: u64,
}

#[table(name = mob, public, index(name = position, btree(columns = [pos_x, pos_y, pos_z])))]
pub struct Mob {
    #[primary_key]
    #[auto_inc]
    id: u64,
    chunk_id: u64, // FK ChunkData.id
    pos_x: f32,
    pos_y: f32,
    pos_z: f32,
    vel_azi: f32,
    vel_rot: f32,
    vel_mag: f32,
}

#[reducer]
fn make_chunk(ctx: &ReducerContext) -> Result<(), String> {
    let chunk_data = ctx.db.chunk_data();
    for _chunk in chunk_data.position().filter((0, 0, 0)) {
        return Err("Already have our one chunk".to_string());
    }
    let new_row = chunk_data.insert(ChunkData {
        id: 0,
        x: 0,
        y: 0,
        z: 0,
    });
    log::info!("Inserted new Chunk {}", new_row.id);
    Ok(())
}

#[reducer]
fn make_node(ctx: &ReducerContext) -> Result<(), String> {
    let chunk = get_chunk(&ctx.db, 0, 0, 0).or_else(|msg| Err(msg)).unwrap();
    for z in -1..=1 {
        for y in -1..=1 {
            for x in -1..=1 {
                let _ = insert_node(
                    &ctx.db,
                    NodeData {
                        id: 0,
                        chunk_id: chunk.id,
                        x,
                        y,
                        z,
                        contents: [].to_vec(),
                    },
                )
                .or_else(|msg| Err(msg));
            }
        }
    }
    Ok(())
}

fn get_chunk(db: &Local, x: i32, y: i32, z: i32) -> Result<ChunkData, String> {
    for chunk in db.chunk_data().position().filter((x, y, z)) {
        return Ok(chunk);
    }
    Err(format!("Chunk not found at ({}, {}, {})", x, y, z))
}

fn insert_node(db: &Local, node: NodeData) -> Result<u64, String> {
    for _chunk in db.node_data().position().filter((node.x, node.y, node.z)) {
        return Err("Already a chunk at that position".to_string());
    }
    assert!(node.id == 0, "Insert node id must be 0");
    let new_row = db.node_data().insert(node);
    Ok(new_row.id)
}
