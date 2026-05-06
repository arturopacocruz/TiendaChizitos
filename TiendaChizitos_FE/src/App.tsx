import { useState, useEffect } from 'react';
import axios from 'axios';
// Importamos la interfaz que creamos en el Paso 1
import { type Cliente } from './lib/types/definicion'; 

// Importaciones de Material UI
import { 
  Container, Table, TableBody, TableCell, TableContainer, 
  TableHead, TableRow, Paper, Typography, Chip, Button, Box 
} from '@mui/material';

function App() {
  const [clientes, setClientes] = useState<Cliente[]>([]);

  useEffect(() => {
    // Llamada a tu API de .NET
    axios.get('http://localhost:5001/api/clientes')
      .then(response => {
        setClientes(response.data);
      })
      .catch(error => {
        console.error('Error al obtener clientes:', error);
      });
  }, []);

  return (
    <Container maxWidth="lg" sx={{ mt: 5 }}>
      <Typography variant="h4" sx={{ mb: 3, fontWeight: 'bold' }}>
        Panel de Clientes - Tienda Chizitos
      </Typography>

      <TableContainer component={Paper} elevation={3}>
        <Table>
          <TableHead sx={{ backgroundColor: '#1976d2' }}>
            <TableRow>
              <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>CI</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Nombre</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Frecuente</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Descuento</TableCell>
              <TableCell sx={{ color: 'white', fontWeight: 'bold' }}>Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {clientes.map((c) => (
              <TableRow key={c.id} hover>
                <TableCell>{c.ci} {c.extension}</TableCell>
                <TableCell>{c.nombre}</TableCell>
                <TableCell>
                  <Chip 
                    label={c.esFrecuente ? "Sí" : "No"} 
                    color={c.esFrecuente ? "success" : "default"} 
                  />
                </TableCell>
                <TableCell>{c.porcentajeDescuento}%</TableCell>
                <TableCell>
                  <Button variant="outlined" size="small">Detalles</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
}

export default App;