import { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import { 
  Container, Table, TableBody, TableCell, TableContainer, 
  TableHead, TableRow, Paper, Typography, Chip, Button, Box, TextField, Alert, Stack 
} from '@mui/material';

// Importamos los tipos
import type { Cliente, AñadirClienteIN, ActualizarClienteIN, BuscarParams } from './lib/types/definicion';

const API_URL = 'http://localhost:5001/api/clientes';

function App() {
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [mensaje, setMensaje] = useState<{tipo: 'success' | 'error', texto: string} | null>(null);
  
  // Estados para formularios
  const [filtros, setFiltros] = useState<BuscarParams>({ nombre: '', ci: undefined, extension: '' });
  const [nuevo, setNuevo] = useState<AñadirClienteIN>({ nombre: '', ci: 0, extension: '', fechaNacimiento: '' });
  const [editando, setEditando] = useState<(ActualizarClienteIN & { id: string }) | null>(null);

  // Referencias para navegación
  const refListar = useRef<HTMLElement>(null);
  const refBuscar = useRef<HTMLElement>(null);
  const refAgregar = useRef<HTMLElement>(null);
  const refEditar = useRef<HTMLElement>(null);

  const irA = (ref: React.RefObject<HTMLElement | null>) => {
    if (ref.current) {
      ref.current.scrollIntoView({ behavior: 'smooth' });
    }
  };

  // --- LÓGICA DE ENDPOINTS ---

  const cargarClientes = async () => {
    try {
      const res = await axios.get<Cliente[]>(API_URL);
      setClientes(res.data);
    } catch (err) { console.error(err); }
  };

  const ejecutarBusqueda = async () => {
    const params = new URLSearchParams();
    if (filtros.nombre) params.append('nombre', filtros.nombre);
    if (filtros.ci) params.append('ci', filtros.ci.toString());
    if (filtros.extension) params.append('extension', filtros.extension);
    
    try {
      const res = await axios.get<Cliente[]>(`${API_URL}/Buscar?${params.toString()}`);
      setClientes(res.data);
    } catch (err) { console.error(err); }
  };

  const guardarCliente = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axios.post(`${API_URL}/Agregar`, nuevo);
      setMensaje({ tipo: 'success', texto: "Cliente creado con éxito" });
      cargarClientes();
      setNuevo({ nombre: '', ci: 0, extension: '', fechaNacimiento: '' });
    } catch (err: any) {
      setMensaje({ tipo: 'error', texto: err.response?.data || "Error al crear" });
    }
  };

  const seleccionarParaEditar = async (id: string) => {
    try {
      const res = await axios.get<Cliente>(`${API_URL}/${id}`);
      setEditando({ 
        id: res.data.id,
        nombre: res.data.nombre,
        ci: res.data.ci,
        extension: res.data.extension || '',
        fechaNacimiento: res.data.fechaNacimiento.split('T')[0] 
      });
      setTimeout(() => irA(refEditar), 100);
    } catch (err) { console.error(err); }
  };

  const actualizarCliente = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editando) return;
    try {
      await axios.put(`${API_URL}/${editando.id}`, editando);
      setMensaje({ tipo: 'success', texto: "Cliente actualizado" });
      setEditando(null);
      cargarClientes();
    } catch (err: any) {
      setMensaje({ tipo: 'error', texto: err.response?.data || "Error al actualizar" });
    }
  };

  useEffect(() => { cargarClientes(); }, []);

  return (
    <Container maxWidth="lg" sx={{ py: 5 }}>
      {/* NAVEGACIÓN RÁPIDA */}
      <Paper sx={{ p: 2, mb: 4, position: 'sticky', top: 10, zIndex: 10, border: '1px solid #ddd', bgcolor: 'white' }}>
        <Typography variant="subtitle2" sx={{ fontWeight: 'bold' }} gutterBottom>ENDPOINTS CLIENTES:</Typography>
        <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
          <Button size="small" variant="contained" onClick={() => irA(refBuscar)}>1. Buscar</Button>
          <Button size="small" variant="contained" onClick={() => irA(refAgregar)}>2. Agregar</Button>
          <Button size="small" variant="contained" onClick={() => irA(refEditar)}>3. Editar</Button>
          <Button size="small" variant="contained" onClick={() => irA(refListar)}>4. Listar</Button>
        </Stack>
      </Paper>

      {/* SECCIÓN 1: BUSCAR (AZUL CLARO) */}
      <Box ref={refBuscar} sx={{ mb: 6 }}>
        <Typography variant="h5" sx={{ mb: 2, fontWeight: 'bold' }}>1. GET /api/clientes/Buscar</Typography>
        <Paper sx={{ p: 3, bgcolor: '#e3f2fd', border: '1px solid #90caf9' }}>
            <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap' }}>
              <TextField label="Nombre" size="small" sx={{ bgcolor: 'white' }} value={filtros.nombre} onChange={e => setFiltros({...filtros, nombre: e.target.value})} />
              <TextField label="CI" type="number" size="small" sx={{ bgcolor: 'white' }} value={filtros.ci || ''} onChange={e => setFiltros({...filtros, ci: e.target.value ? parseInt(e.target.value) : undefined})} />
              <TextField label="Extensión" size="small" sx={{ bgcolor: 'white' }} value={filtros.extension} onChange={e => setFiltros({...filtros, extension: e.target.value})} />
              <Button variant="contained" onClick={ejecutarBusqueda}>Ejecutar Buscar</Button>
              <Button variant="text" onClick={() => { setFiltros({nombre:'', ci:undefined, extension: ''}); cargarClientes(); }}>Ver Todos</Button>
            </Stack>
        </Paper>
      </Box>

      {/* SECCIÓN 2: AGREGAR (VERDE CLARO) */}
      <Box ref={refAgregar} sx={{ mb: 6 }}>
        <Typography variant="h5" sx={{ mb: 2, fontWeight: 'bold' }}>2. POST /api/clientes/Agregar</Typography>
        {mensaje && <Alert severity={mensaje.tipo} sx={{ mb: 2 }}>{mensaje.texto}</Alert>}
        <Paper component="form" onSubmit={guardarCliente} sx={{ p: 3, display: 'flex', gap: 2, flexWrap: 'wrap', bgcolor: '#e8f5e9', border: '1px solid #a5d6a7' }}>
          <TextField label="Nombre" required size="small" sx={{ bgcolor: 'white' }} value={nuevo.nombre} onChange={e => setNuevo({...nuevo, nombre: e.target.value})} />
          <TextField label="CI" type="number" required size="small" sx={{ bgcolor: 'white' }} value={nuevo.ci || ''} onChange={e => setNuevo({...nuevo, ci: parseInt(e.target.value) || 0})} />
          <TextField label="Ext." size="small" sx={{ bgcolor: 'white' }} value={nuevo.extension} onChange={e => setNuevo({...nuevo, extension: e.target.value})} />
          <TextField 
            label="F. Nacimiento" type="date" required size="small" sx={{ bgcolor: 'white' }}
            slotProps={{ inputLabel: { shrink: true } }} 
            value={nuevo.fechaNacimiento} onChange={e => setNuevo({...nuevo, fechaNacimiento: e.target.value})} 
          />
          <Button type="submit" variant="contained" color="success">Guardar Cliente</Button>
        </Paper>
      </Box>

      {/* SECCIÓN 3: EDITAR (NARANJA CLARO) */}
      <Box ref={refEditar} sx={{ mb: 6 }}>
        <Typography variant="h5" sx={{ mb: 2, fontWeight: 'bold' }}>3. PUT /api/clientes/ID</Typography>
        {editando ? (
          <Paper component="form" onSubmit={actualizarCliente} sx={{ p: 3, display: 'flex', gap: 2, flexWrap: 'wrap', bgcolor: '#fff3e0', border: '1px solid #ffcc80' }}>
            <TextField label="Nombre" size="small" sx={{ bgcolor: 'white' }} value={editando.nombre} onChange={e => setEditando({...editando, nombre: e.target.value})} />
            <TextField label="CI" type="number" size="small" sx={{ bgcolor: 'white' }} value={editando.ci} onChange={e => setEditando({...editando, ci: parseInt(e.target.value) || 0})} />
            <TextField label="Ext." size="small" sx={{ bgcolor: 'white' }} value={editando.extension} onChange={e => setEditando({...editando, extension: e.target.value})} />
            <TextField 
              label="F. Nacimiento" type="date" size="small" sx={{ bgcolor: 'white' }}
              slotProps={{ inputLabel: { shrink: true } }} 
              value={editando.fechaNacimiento} onChange={e => setEditando({...editando, fechaNacimiento: e.target.value})} 
            />
            <Button type="submit" variant="contained" color="warning">Actualizar</Button>
            <Button variant="outlined" color="error" onClick={() => setEditando(null)}>Cancelar</Button>
          </Paper>
        ) : (
          <Paper sx={{ p: 3, bgcolor: '#fff3e0', border: '1px dashed #ffcc80', textAlign: 'center' }}>
            <Typography variant="body2" color="textSecondary">Selecciona un cliente de la lista para editarlo</Typography>
          </Paper>
        )}
      </Box>

      {/* SECCIÓN 4: LISTAR (ROJO CLARO) */}
      <Box ref={refListar}>
        <Typography variant="h5" sx={{ mb: 2, fontWeight: 'bold' }}>4. GET /api/clientes</Typography>
        <TableContainer component={Paper} sx={{ bgcolor: '#ffebee', border: '1px solid #ef9a9a' }}>
          <Table>
            <TableHead sx={{ bgcolor: '#d32f2f' }}>
              <TableRow>
                <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>CI / Ext</TableCell>
                <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Nombre</TableCell>
                <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Frecuente</TableCell>
                <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Acciones</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {clientes.map((c) => (
                <TableRow key={c.id} hover>
                  <TableCell>{c.ci} {c.extension}</TableCell>
                  <TableCell>{c.nombre}</TableCell>
                  <TableCell>
                    <Chip label={c.esFrecuente ? "Sí" : "No"} color={c.esFrecuente ? "success" : "default"} size="small" />
                  </TableCell>
                  <TableCell>
                    <Button size="small" variant="contained" color="info" onClick={() => seleccionarParaEditar(c.id)}>Editar</Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>
    </Container>
  );
}

export default App;

